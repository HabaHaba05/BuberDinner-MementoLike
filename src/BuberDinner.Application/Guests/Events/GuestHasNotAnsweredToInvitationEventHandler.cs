using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class GuestHasNotAnsweredToInvitationEventHandler : INotificationHandler<GuestHasNotAnsweredToInvitation>
{
    private readonly IUnitOfWork _unitOfWork;

    public GuestHasNotAnsweredToInvitationEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(GuestHasNotAnsweredToInvitation guestHasNotAnsweredEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(guestHasNotAnsweredEvent.Reservation.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest not found. Guest Id: {guestHasNotAnsweredEvent.Reservation.GuestId}");
        }

        guest.ChangeReservationToFinished(guestHasNotAnsweredEvent.Reservation.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}