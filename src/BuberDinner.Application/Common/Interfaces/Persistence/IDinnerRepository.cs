using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;

public interface IDinnerRepository
{
    Task AddAsync(Dinner dinner);
    Task<List<Dinner>> ListAsync(HostId hostId);
}