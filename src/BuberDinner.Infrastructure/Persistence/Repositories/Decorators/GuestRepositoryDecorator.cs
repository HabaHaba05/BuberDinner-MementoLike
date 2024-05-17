using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class GuestRepositoryDecorator : IGuestRepository
{
    private readonly IdentityMap _map;
    private readonly IGuestRepository _guestRepository;
    public GuestRepositoryDecorator(IdentityMap map, IGuestRepository guestRepository)
    {
        _map = map;
        _guestRepository = guestRepository;
    }

    public async Task AddAsync(Guest guest)
    {
        await _guestRepository.AddAsync(guest);
        _map.AddNew(guest);
    }

    public async Task<Guest?> GetByIdAsync(GuestId guestId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", guestId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Guest)result;
        }

        var guest = await _guestRepository.GetByIdAsync(guestId);
        if (guest is not null)
        {
            _map.AddAlreadyExisting(guest);
        }

        return guest;
    }

    public async Task<List<Guest>> GetByIdsAsync(List<GuestId> guestIds)
    {
        var guests = await _guestRepository.GetByIdsAsync(guestIds);
        guests.ForEach(x => _map.AddAlreadyExisting(x));
        return guests;
    }

    public async Task<GuestId?> GetGuestIdOfUserAsync(UserId userId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("UserId", userId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return ((Guest)result).Id;
        }

        var guestId = await _guestRepository.GetGuestIdOfUserAsync(userId);

        return guestId;
    }

    public async Task UpdateAsync(Guest guest)
    {
        await _guestRepository.UpdateAsync(guest);
        _map.AddAlreadyExisting(guest);
    }

    public async Task UpdateRangeAsync(List<Guest> guests)
    {
        await _guestRepository.UpdateRangeAsync(guests);
        guests.ForEach(g => _map.AddAlreadyExisting(g));
    }
}