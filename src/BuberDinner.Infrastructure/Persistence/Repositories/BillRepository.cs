using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate;
using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class BillRepository : RepositoryBase, IBillRepository
{
    private readonly IdentityMap _identityMap;

    public BillRepository(
        IdentityMap map,
        IDbTransaction transaction) : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(Bill bill)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(bill.GetState(), Bill.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<Bill?> GetByIdAsync(BillId id)
    {
        var queryWithParams = SqlQueryBuilder.Select(Bill.TableName, "*", [new("Id", id.Value)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : Bill.FromState(queryResult);
    }

    public async Task UpdateAsync(Bill bill)
    {
        var currentState = bill.GetState();
        var previousState = _identityMap.FindByKeys(bill)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, Bill.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}