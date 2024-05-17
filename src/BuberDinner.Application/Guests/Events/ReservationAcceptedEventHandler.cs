using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class ReservationAcceptedEventHandler : INotificationHandler<ReservationAccepted>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationAcceptedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationAccepted reservationAcceptedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(reservationAcceptedEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. GuestId: {reservationAcceptedEvent.Reservation.GuestId}");
        }

        guest.AddUpcomingReservation(reservationAcceptedEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}