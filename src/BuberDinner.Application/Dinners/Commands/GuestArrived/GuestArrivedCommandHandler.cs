using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.GuestArrived;

public class GuestArrivedCommandHandler : IRequestHandler<GuestArrivedCommand, ErrorOr<Dinner>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GuestArrivedCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Dinner>> Handle(GuestArrivedCommand request, CancellationToken cancellationToken)
    {
        var dinnerId = DinnerId.Create(request.DinnerId);
        if (dinnerId.IsError)
        {
            return dinnerId.Errors;
        }

        var guestId = GuestId.Create(request.GuestId);
        if (dinnerId.IsError)
        {
            return dinnerId.Errors;
        }

        if (await _unitOfWork.DinnerRepository.GetByIdAsync(dinnerId.Value) is not Dinner dinner)
        {
            return Errors.Dinner.NotFound;
        }

        var guestArrivedResult = dinner.GuestArrived(request.HostId, guestId.Value);
        if (guestArrivedResult.IsError)
        {
            return guestArrivedResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return dinner;
    }
}