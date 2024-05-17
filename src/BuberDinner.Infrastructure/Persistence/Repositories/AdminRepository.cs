using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.AdminAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class AdminRepository : RepositoryBase, IAdminRepository
{
    public AdminRepository(IDbTransaction transaction)
        : base(transaction)
    {
    }

    public async Task<Admin?> GetAdminByEmailAsync(string email)
    {
        var queryWithParams = SqlQueryBuilder.Select(Admin.TableName, "*", [new("Email", email)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : Admin.FromState(queryResult);
    }
}