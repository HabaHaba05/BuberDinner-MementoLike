using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Guests.Commands;
public class CreateGuestRatingCommandHandler : IRequestHandler<CreateGuestRatingCommand, ErrorOr<Guest>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuestRatingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guest>> Handle(CreateGuestRatingCommand request, CancellationToken cancellationToken)
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

        if (await _unitOfWork.GuestRepository.GetByIdAsync(guestId.Value) is not Guest guest)
        {
            return Errors.Guest.NotFound;
        }

        if (dinner.HostId != request.HostId)
        {
            return Error.Forbidden(description: "Can not post guestRating when dinner was not created by you");
        }

        if (!dinner.CompletedReservations.Any(x => x.GuestId == guestId.Value))
        {
            return Error.NotFound(description: "Couldn't find guest' reservation for the dinner");
        }

        guest.AddRating(request.HostId, dinnerId.Value, request.Rating);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);

        return guest;
    }
}