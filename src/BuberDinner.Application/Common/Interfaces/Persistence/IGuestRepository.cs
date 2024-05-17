using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;

public interface IGuestRepository
{
    Task<GuestId?> GetGuestIdOfUserAsync(UserId userId);
    Task AddAsync(Guest guest);
    Task<Guest?> GetByIdAsync(GuestId guestId);
    Task<List<Guest>> GetByIdsAsync(List<GuestId> guestIds);
    Task UpdateAsync(Guest guest);
    Task UpdateRangeAsync(List<Guest> guests);
}