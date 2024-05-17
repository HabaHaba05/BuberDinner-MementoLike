using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.AcceptDinnerInvitation;
public class AccepDinnerInvitationCommandHandler : IRequestHandler<AcceptDinnerInvitationCommand, ErrorOr<Reservation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AccepDinnerInvitationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Reservation>> Handle(AcceptDinnerInvitationCommand request, CancellationToken cancellationToken)
    {
        var reservationId = ReservationId.Create(request.ReservationId);
        if (reservationId.IsError)
        {
            return reservationId.Errors;
        }

        if (await _unitOfWork.DinnerRepository.GetByReservationIdAsync(reservationId.Value) is not Dinner dinner)
        {
            return Errors.Dinner.DinnerNotFoundByReservationId;
        }

        var acceptReservationInvitationResult = dinner.AcceptReservationInvitation(reservationId.Value, request.GuestId);

        if (acceptReservationInvitationResult.IsError)
        {
            return acceptReservationInvitationResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return acceptReservationInvitationResult.Value;
    }
}