using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.InviteGuestToDinner;

public class InviteGuestToDinnerCommandHandler : IRequestHandler<InviteGuestToDinnerCommand, ErrorOr<Reservation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public InviteGuestToDinnerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Reservation>> Handle(InviteGuestToDinnerCommand request, CancellationToken cancellationToken)
    {
        var dinnerId = DinnerId.Create(request.DinnerId);
        if (dinnerId.IsError)
        {
            return dinnerId.Errors;
        }

        var guestId = GuestId.Create(request.GuestId);
        if (guestId.IsError)
        {
            return guestId.Errors;
        }

        if (await _unitOfWork.DinnerRepository.GetByIdAsync(dinnerId.Value) is not Dinner dinner)
        {
            return Errors.Dinner.NotFound;
        }

        var reservation = dinner.InviteGuest(guestId.Value, request.HostId);

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return reservation;
    }
}