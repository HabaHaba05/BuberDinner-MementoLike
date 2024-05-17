using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class GuestHasNotArrivedToDinnerEventHandler : INotificationHandler<GuestHasNotArrivedToDinner>
{
    private readonly IUnitOfWork _unitOfWork;

    public GuestHasNotArrivedToDinnerEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(GuestHasNotArrivedToDinner guestHasNotArrivedToDinnerEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(guestHasNotArrivedToDinnerEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest not found. Guest Id: {guestHasNotArrivedToDinnerEvent.Reservation.GuestId}");
        }

        guest.ChangeReservationToFinished(guestHasNotArrivedToDinnerEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}