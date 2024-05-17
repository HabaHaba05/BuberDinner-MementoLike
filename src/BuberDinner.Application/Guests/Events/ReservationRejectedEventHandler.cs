using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class ReservationRejectedEventHandler : INotificationHandler<ReservationRejected>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationRejectedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationRejected reservationRejectedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(reservationRejectedEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. GuestId: {reservationRejectedEvent.Reservation.GuestId}");
        }

        guest.ChangeReservationToFinished(reservationRejectedEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}