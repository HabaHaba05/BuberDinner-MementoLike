using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class ReservationCompletedEventHandler : INotificationHandler<ReservationCompleted>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCompletedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationCompleted reservationCompletedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(reservationCompletedEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. GuestId: {reservationCompletedEvent.Reservation.GuestId}");
        }

        guest.ChangeReservationToFinished(reservationCompletedEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}