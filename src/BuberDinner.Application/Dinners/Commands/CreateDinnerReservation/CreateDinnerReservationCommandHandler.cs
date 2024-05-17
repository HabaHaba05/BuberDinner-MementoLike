using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CreateDinnerReservation;

public class CreateDinnerReservationCommandHandler : IRequestHandler<CreateDinnerReservationCommand, ErrorOr<Reservation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDinnerReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Reservation>> Handle(CreateDinnerReservationCommand request, CancellationToken cancellationToken)
    {
        var dinnerId = DinnerId.Create(request.DinnerId);
        if (dinnerId.IsError)
        {
            return dinnerId.Errors;
        }

        if (await _unitOfWork.DinnerRepository.GetByIdAsync(dinnerId.Value) is not Dinner dinner)
        {
            return Errors.Dinner.NotFound;
        }

        var addReservationResponse = dinner.AddReservation(request.GuestId, request.GuestsCount);
        if (addReservationResponse.IsError)
        {
            return addReservationResponse.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return addReservationResponse.Value;
    }
}