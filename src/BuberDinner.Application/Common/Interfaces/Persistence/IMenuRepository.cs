using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;

public interface IMenuRepository
{
    Task UpdateAsync(Menu menu);
    Task AddAsync(Menu menu);
    Task<Menu?> GetByIdAsync(MenuId menuId);
    Task<bool> ExistsAsync(MenuId menuId);
    Task<List<Menu>> ListAsync(HostId hostId);
}