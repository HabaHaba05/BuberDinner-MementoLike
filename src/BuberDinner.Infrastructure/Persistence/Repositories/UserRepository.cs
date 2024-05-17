using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.UserAggregate;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(IDbTransaction transaction)
        : base(transaction)
    {
    }

    public async Task AddAsync(User user)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(user.GetState(), User.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var queryWithParams = SqlQueryBuilder.Select(User.TableName, "*", [new("Email", email)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : User.FromState(queryResult);
    }

    public async Task<User?> GetUserByIdAsync(UserId userId)
    {
        var queryWithParams = SqlQueryBuilder.Select(User.TableName, "*", [new("Id", userId.Value)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : User.FromState(queryResult);
    }
}