using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.BecomeHostRequestAggregate.Enums;
using BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class BecomeHostRequestRepository : RepositoryBase, IBecomeHostRequestRepository
{
    private readonly IdentityMap _identityMap;
    public BecomeHostRequestRepository(
        IdentityMap map,
        IDbTransaction transaction)
        : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(BecomeHostRequest becomeHostRequest)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(becomeHostRequest.GetState(), BecomeHostRequest.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<BecomeHostRequest?> GetByIdAsync(BecomeHostRequestId becomeHostRequestId)
    {
        var queryWithParams = SqlQueryBuilder.Select(BecomeHostRequest.TableName, "*", [new("Id", becomeHostRequestId.Value)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : BecomeHostRequest.FromState(queryResult);
    }

    public async Task<List<BecomeHostRequest>> GetByUserIdAsync(UserId userId)
    {
        var queryWithParams = SqlQueryBuilder.Select(BecomeHostRequest.TableName, "*", [new("UserId", userId.Value)]);
        var queryResult = (await Connection.QueryAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))
            .ToList()
            .Select(x => ((IDictionary<string, object?>)x).ToDictionary(x => x.Key, x => x.Value))
            .ToList();

        return queryResult is null
            ? []
            : queryResult.Select(BecomeHostRequest.FromState).ToList();
    }

    public async Task<List<BecomeHostRequest>> ListPendingRequestsAsync()
    {
        var queryWithParams = SqlQueryBuilder.Select(BecomeHostRequest.TableName, "*", [new("Status", RequestResponseType.Pending.Value)]);
        var queryResult = (await Connection.QueryAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))
            .ToList()
            .Select(x => ((IDictionary<string, object?>)x).ToDictionary(x => x.Key, x => x.Value))
            .ToList();

        return queryResult is null
            ? []
            : queryResult.Select(BecomeHostRequest.FromState).ToList();
    }

    public async Task UpdateAsync(BecomeHostRequest becomeHostRequest)
    {
        var currentState = becomeHostRequest.GetState();
        var previousState = _identityMap.FindByKeys(becomeHostRequest)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, BecomeHostRequest.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}