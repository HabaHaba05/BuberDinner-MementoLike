using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;

public interface IHostRepository
{
    Task<Host?> GetByIdAsync(HostId hostId);
    Task<HostId?> GetHostIdOfUserAsync(UserId userId);
    Task AddAsync(Host host);
    Task UpdateAsync(Host host);
}