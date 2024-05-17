using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.MenuReviewAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Guests.Events;
public class MenuReviewCreatedEventHandler : INotificationHandler<MenuReviewCreated>
{
    private readonly IUnitOfWork _unitOfWork;
    public MenuReviewCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MenuReviewCreated notification, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(notification.MenuReview.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest not found. Guest id: {notification.MenuReview.GuestId}).");
        }

        guest.AddMenuReview(notification.MenuReview.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}