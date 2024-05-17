using BuberDinner.Domain.AdminAggregate;

namespace BuberDinner.Application.Common.Interfaces.Persistence;
public interface IAdminRepository
{
    Task<Admin?> GetAdminByEmailAsync(string email);
}