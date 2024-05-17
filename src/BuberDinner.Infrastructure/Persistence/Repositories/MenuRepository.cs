using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class MenuRepository : RepositoryBase, IMenuRepository
{
    private readonly IdentityMap _identityMap;

    public MenuRepository(
        IdentityMap map,
        IDbTransaction transaction)
        : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(Menu menu)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(menu.GetState(), Menu.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<bool> ExistsAsync(MenuId menuId)
    {
        var queryWithParams = SqlQueryBuilder.Select(Menu.TableName, "COUNT(*)", [new("Id", menuId.Value)]);
        var queryResult = await Connection
            .ExecuteScalarAsync<bool>(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);

        return queryResult;
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId)
    {
        var query = @$"
            SELECT * FROM Menus WHERE Id = @MenuId;
	        SELECT * FROM MenuSections WHERE MenuId = @MenuId;
            SELECT * FROM MenuItems WHERE MenuId = @MenuId;
	        SELECT DinnerId FROM MenuDinnerIds WHERE MenuId = @MenuId;
            SELECT ReviewId FROM MenuReviewIds WHERE MenuId = @MenuId;
        ";

        var queryResult = await Connection.QueryMultipleAsync(query, new { MenuId = menuId.Value }, transaction: Transaction);
        var menu = ((IDictionary<string, object?>)queryResult.ReadFirst()).ToDictionary(x => x.Key, x => x.Value);
        var menuSections = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var menuItems = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var dinnerIds = queryResult.Read<Guid>().ToList();
        var reviewIds = queryResult.Read<Guid>().ToList();

        if (menu is not null)
        {
            foreach (var menuSection in menuSections)
            {
                var items = menuItems.Where(x => x["MenuSectionId"]!.ToString() == menuSection["MenuSectionId"]!.ToString()).ToList();
                menuSection.Add("MenuItems", items);
            }

            menu.Add("MenuSections", menuSections);
            menu.Add("MenuDinnerIds", dinnerIds);
            menu.Add("MenuReviewIds", reviewIds);
            return Menu.FromState(menu);
        }

        return null;
    }

    public async Task<List<Menu>> ListAsync(HostId hostId)
    {
        var query = @$"
            SELECT * FROM Menus WHERE HostId = @HostId;
	        SELECT * FROM MenuSections WHERE 
                MenuId IN (SELECT Id FROM Menus WHERE HostId = @HostId);
            SELECT * FROM MenuItems WHERE
                MenuId IN (SELECT Id FROM Menus WHERE HostId = @HostId);
	        SELECT * FROM MenuDinnerIds WHERE
                MenuId IN (SELECT Id FROM Menus WHERE HostId = @HostId);
            SELECT * FROM MenuReviewIds WHERE
                MenuId IN (SELECT Id FROM Menus WHERE HostId = @HostId);
        ";

        var queryResult = await Connection.QueryMultipleAsync(query, new { HostId = hostId.Value }, transaction: Transaction);
        var menus = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var menuSections = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var menuItems = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var dinnerIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var reviewIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();

        foreach (var menuSection in menuSections)
        {
            var items = menuItems.Where(x => x["MenuSectionId"]!.ToString() == menuSection["MenuSectionId"]!.ToString()).ToList();
            menuSection.Add("MenuItems", items);
        }

        foreach (var menu in menus)
        {
            var sections = menuSections.Where(x => x["MenuId"]!.ToString() == menu["Id"]!.ToString()).ToList();
            menu.Add("MenuSections", sections);

            var menuDinnerIds = dinnerIds.Where(x => x["MenuId"]!.ToString() == menu["Id"]!.ToString()).Select(x => (Guid)x["DinnerId"]!).ToList();
            var menuReviewIds = reviewIds.Where(x => x["MenuId"]!.ToString() == menu["Id"]!.ToString()).Select(x => (Guid)x["ReviewId"]!).ToList();
            menu.Add("MenuDinnerIds", menuDinnerIds);
            menu.Add("MenuReviewIds", menuReviewIds);
        }

        return menus.Count == 0
            ? []
            : menus.Select(Menu.FromState).ToList();
    }

    public async Task UpdateAsync(Menu menu)
    {
        var currentState = menu.GetState();
        var previousState = _identityMap.FindByKeys(menu)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, Menu.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}