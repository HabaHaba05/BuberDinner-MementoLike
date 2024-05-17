using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class MenuRepositoryDecorator : IMenuRepository
{
    private readonly IdentityMap _map;
    private readonly IMenuRepository _menuRepository;
    public MenuRepositoryDecorator(IdentityMap map, IMenuRepository menuRepository)
    {
        _map = map;
        _menuRepository = menuRepository;
    }

    public async Task AddAsync(Menu menu)
    {
        await _menuRepository.AddAsync(menu);
        _map.AddNew(menu);
    }

    public Task<bool> ExistsAsync(MenuId menuId)
    {
        return _menuRepository.ExistsAsync(menuId);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", menuId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Menu)result;
        }

        var menu = await _menuRepository.GetByIdAsync(menuId);
        if (menu is not null)
        {
            _map.AddAlreadyExisting(menu);
        }

        return menu;
    }

    public async Task<List<Menu>> ListAsync(HostId hostId)
    {
        var menus = await _menuRepository.ListAsync(hostId);
        menus.ForEach(x => _map.AddAlreadyExisting(x));
        return menus;
    }

    public async Task UpdateAsync(Menu menu)
    {
        await _menuRepository.UpdateAsync(menu);
        _map.AddAlreadyExisting(menu);
    }
}