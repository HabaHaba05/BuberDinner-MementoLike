using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.AdminAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class AdminRepositoryDecorator : IAdminRepository
{
    private readonly IdentityMap _map;
    private readonly IAdminRepository _adminRepository;
    public AdminRepositoryDecorator(IdentityMap map, IAdminRepository adminRepository)
    {
        _map = map;
        _adminRepository = adminRepository;
    }

    public async Task<Admin?> GetAdminByEmailAsync(string email)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Email", email)]).FirstOrDefault();

        if (result is not null)
        {
            return (Admin)result;
        }

        var admin = await _adminRepository.GetAdminByEmailAsync(email);

        if (admin is not null)
        {
            _map.AddAlreadyExisting(admin);
        }

        return admin;
    }
}