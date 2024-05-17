using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.RejectDinnerInvitation;

public class RejectDinnerInvitationCommandHandler : IRequestHandler<RejectDinnerInvitationCommand, ErrorOr<Reservation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectDinnerInvitationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Reservation>> Handle(RejectDinnerInvitationCommand request, CancellationToken cancellationToken)
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

        var rejectReservationInvitationResult = dinner.RejectReservationInvitation(reservationId.Value, request.GuestId);

        if (rejectReservationInvitationResult.IsError)
        {
            return rejectReservationInvitationResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return rejectReservationInvitationResult.Value;
    }
}