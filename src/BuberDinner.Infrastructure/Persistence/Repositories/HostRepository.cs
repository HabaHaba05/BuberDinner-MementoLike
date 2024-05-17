using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class HostRepository : RepositoryBase, IHostRepository
{
    private readonly IdentityMap _identityMap;
    public HostRepository(
        IdentityMap map,
        IDbTransaction transaction)
        : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(Host host)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(host.GetState(), Host.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<Host?> GetByIdAsync(HostId hostId)
    {
        var query = @$"
            SELECT * FROM Hosts WHERE Id = @HostId;
	        SELECT HostMenuId FROM HostMenuIds WHERE HostId = @HostId;
            SELECT HostDinnerId FROM HostDinnerIds WHERE HostId = @HostId;
        ";

        var queryResult = await Connection.QueryMultipleAsync(query, new { HostId = hostId.Value }, transaction: Transaction);
        var host = ((IDictionary<string, object?>?)queryResult.ReadFirst())?.ToDictionary(x => x.Key, x => x.Value);
        var menus = queryResult.Read<Guid>();
        var dinners = queryResult.Read<Guid>();

        if (host is not null)
        {
            host["HostMenuIds"] = menus;
            host["HostDinnerIds"] = dinners;
        }

        return host is null ? null : Host.FromState(host);
    }

    public async Task<HostId?> GetHostIdOfUserAsync(UserId userId)
    {
        var queryWithParams = SqlQueryBuilder.Select(Host.TableName, "Id", [new("UserId", userId.Value)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : HostId.Create((Guid)queryResult["Id"]!);
    }

    public async Task UpdateAsync(Host host)
    {
        var currentState = host.GetState();
        var previousState = _identityMap.FindByKeys(host)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, Host.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}