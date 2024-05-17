using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CancelReservation;

public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ErrorOr<Reservation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Reservation>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
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

        var cancelReservationResult = dinner.CancelReservation(reservationId.Value, request.GuestId);

        if (cancelReservationResult.IsError)
        {
            return cancelReservationResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return cancelReservationResult.Value;
    }
}