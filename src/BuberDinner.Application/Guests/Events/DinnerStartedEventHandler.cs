using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Enums;
using BuberDinner.Domain.DinnerAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class DinnerStartedEventHandler : INotificationHandler<DinnerStarted>
{
    private readonly IUnitOfWork _unitOfWork;

    public DinnerStartedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DinnerStarted dinnerStartedEvent, CancellationToken cancellationToken)
    {
        var guestIdWithReservationId = dinnerStartedEvent.Dinner.Reservations
            .Where(x => x.Status == ReservationStatus.Reserved)
            .Select(x => new { x.GuestId, ReservationId = x.Id });
        if (guestIdWithReservationId.Any())
        {
            var guests = await _unitOfWork.GuestRepository.GetByIdsAsync(guestIdWithReservationId.Select(x => x.GuestId).ToList());
            guests.ForEach(guest =>
                guest.ChangeReservationToOngoing(guestIdWithReservationId
                    .Single(x => x.GuestId == guest.Id).ReservationId));
            await _unitOfWork.GuestRepository.UpdateRangeAsync(guests);
        }
    }
}