using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;

public interface IDinnerRepository
{
    Task AddAsync(Dinner dinner);
    Task<List<Dinner>> ListAsync(HostId hostId);
    Task<Dinner?> GetByIdAsync(DinnerId dinnerId);
    Task<Dinner?> GetByReservationIdAsync(ReservationId reservationId);
    Task UpdateAsync(Dinner dinner);
}