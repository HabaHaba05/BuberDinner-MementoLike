using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class DinnerRepositoryDecorator : IDinnerRepository
{
    private readonly IdentityMap _map;
    private readonly IDinnerRepository _dinnerRepository;
    public DinnerRepositoryDecorator(IdentityMap map, IDinnerRepository dinnerRepository)
    {
        _map = map;
        _dinnerRepository = dinnerRepository;
    }

    public async Task AddAsync(Dinner dinner)
    {
        await _dinnerRepository.AddAsync(dinner);
        _map.AddNew(dinner);
    }

    public async Task<Dinner?> GetByIdAsync(DinnerId dinnerId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", dinnerId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Dinner)result;
        }

        var dinner = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (dinner is not null)
        {
            _map.AddAlreadyExisting(dinner);
        }

        return dinner;
    }

    public async Task<Dinner?> GetByReservationIdAsync(ReservationId reservationId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("ReservationId", reservationId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Dinner)result;
        }

        var dinner = await _dinnerRepository.GetByReservationIdAsync(reservationId);
        if (dinner is not null)
        {
            _map.AddAlreadyExisting(dinner);
        }

        return dinner;
    }

    public async Task<List<Dinner>> ListAsync(HostId hostId)
    {
        var dinners = await _dinnerRepository.ListAsync(hostId);
        dinners.ForEach(x => _map.AddAlreadyExisting(x));
        return dinners;
    }

    public async Task UpdateAsync(Dinner dinner)
    {
        await _dinnerRepository.UpdateAsync(dinner);
        _map.AddAlreadyExisting(dinner);
    }
}