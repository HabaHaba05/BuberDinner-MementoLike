using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.MenuReviewAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Hosts.Events;
public class MenuReviewCreatedEventHandler : INotificationHandler<MenuReviewCreated>
{
    private readonly IUnitOfWork _unitOfWork;
    public MenuReviewCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MenuReviewCreated notification, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.HostRepository.GetByIdAsync(notification.MenuReview.HostId) is not Host host)
        {
            throw new InvalidOperationException($"Host not found. Host Id: {notification.MenuReview.HostId}).");
        }

        host.AddNewRating(notification.MenuReview.Rating);
        await _unitOfWork.HostRepository.UpdateAsync(host);
    }
}