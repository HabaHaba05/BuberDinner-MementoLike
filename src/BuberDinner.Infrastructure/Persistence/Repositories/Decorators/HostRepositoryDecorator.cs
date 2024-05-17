using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class HostRepositoryDecorator : IHostRepository
{
    private readonly IdentityMap _map;
    private readonly IHostRepository _hostRepository;
    public HostRepositoryDecorator(IdentityMap map, IHostRepository hostRepository)
    {
        _map = map;
        _hostRepository = hostRepository;
    }

    public async Task AddAsync(Host host)
    {
        await _hostRepository.AddAsync(host);
        _map.AddNew(host);
    }

    public async Task<Host?> GetByIdAsync(HostId hostId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", hostId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Host)result;
        }

        var host = await _hostRepository.GetByIdAsync(hostId);
        if (host is not null)
        {
            _map.AddAlreadyExisting(host);
        }

        return host;
    }

    public Task<HostId?> GetHostIdOfUserAsync(UserId userId)
    {
        return _hostRepository.GetHostIdOfUserAsync(userId);
    }

    public async Task UpdateAsync(Host host)
    {
        await _hostRepository.UpdateAsync(host);
        _map.AddAlreadyExisting(host);
    }
}