using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class ReservationCancelledEventHandler : INotificationHandler<ReservationCancelled>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCancelledEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationCancelled reservationCancelledEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(reservationCancelledEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. GuestId: {reservationCancelledEvent.Reservation.GuestId}");
        }

        guest.ChangeReservationToFinished(reservationCancelledEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}