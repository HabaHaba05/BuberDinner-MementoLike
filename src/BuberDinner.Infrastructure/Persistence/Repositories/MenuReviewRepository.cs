using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.MenuReviewAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class MenuReviewRepository : RepositoryBase, IMenuReviewRepository
{
    public MenuReviewRepository(IDbTransaction transaction) : base(transaction)
    {
    }

    public async Task AddAsync(MenuReview menuReview)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(menuReview.GetState(), MenuReview.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}