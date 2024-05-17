using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.MenuReviewAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.MenuReviews.Commands.CreateMenuReview;

public class CreateMenuReviewCommandHandler : IRequestHandler<CreateMenuReviewCommand, ErrorOr<MenuReview>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMenuReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<MenuReview>> Handle(CreateMenuReviewCommand request, CancellationToken cancellationToken)
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

        if (await _unitOfWork.GuestRepository.GetByIdAsync(request.GuestId) is not Guest guest)
        {
            return Errors.Guest.NotFound;
        }

        if (!dinner.CompletedReservations.Any(x => x.GuestId == request.GuestId))
        {
            return Error.NotFound(description: "Couldn't find guest' reservation for the dinner");
        }

        var review = MenuReview.Create(
            request.Rating,
            request.Comment,
            dinner.HostId,
            dinner.MenuId,
            request.GuestId,
            dinner.Id);

        await _unitOfWork.MenuReviewRepository.AddAsync(review);

        return review;
    }
}