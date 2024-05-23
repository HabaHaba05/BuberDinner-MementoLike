using System.Runtime.InteropServices;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuAggregate.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace BuberDinner.Infrastructure.Persistence.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly BuberDinnerDbContext _dbContext;

    public MenuRepository(BuberDinnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Menu menu)
    {
        _dbContext.Add(menu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(MenuId menuId)
    {
        return await _dbContext.Menus.AnyAsync(menu => menu.Id == menuId);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId)
    {
        return await _dbContext.Menus.FirstOrDefaultAsync(menu => menu.Id == menuId);
    }

    public async Task<List<Menu>> ListAsync(HostId hostId)
    {
        return await _dbContext.Menus.Where(menu => menu.HostId == hostId).ToListAsync();
    }

    public async Task UpdateAsync(Menu menu)
    {
        _dbContext.Menus.Update(menu);
        await _dbContext.SaveChangesAsync();
    }
}