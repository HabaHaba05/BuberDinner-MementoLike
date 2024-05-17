using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Enums;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class ReservationCreatedEventHandler : INotificationHandler<ReservationCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationCreated reservationCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(reservationCreatedEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. GuestId: {reservationCreatedEvent.Reservation.GuestId}");
        }

        var reservationStatus = reservationCreatedEvent.Reservation.Status;

        if (reservationStatus == ReservationStatus.PendingGuestApproval)
        {
            guest.AddPendingReservation(reservationCreatedEvent.Reservation.Id);
        }
        else if (reservationStatus == ReservationStatus.Reserved)
        {
            guest.AddUpcomingReservation(reservationCreatedEvent.Reservation.Id);
        }

        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}