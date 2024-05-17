using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuReviewAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Menus.Events;
public class MenuReviewCreatedEventHandler : INotificationHandler<MenuReviewCreated>
{
    private readonly IUnitOfWork _unitOfWork;
    public MenuReviewCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MenuReviewCreated notification, CancellationToken cancellationToken)
    {
        if ((await _unitOfWork.MenuRepository.GetByIdAsync(notification.MenuReview.MenuId)) is not Menu menu)
        {
            throw new InvalidOperationException($"Menu not found. Menu id: {notification.MenuReview.MenuId}).");
        }

        menu.AddReview(notification.MenuReview.Rating, notification.MenuReview.Id);
        await _unitOfWork.MenuRepository.UpdateAsync(menu);
    }
}