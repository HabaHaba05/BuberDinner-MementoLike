using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate.Events;
using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.UserAggregate;

using MediatR;

namespace BuberDinner.Application.Hosts.Events;

public class BecomeHostRequestApprovedEventHandler : INotificationHandler<BecomeHostRequestApproved>
{
    private readonly IUnitOfWork _unitOfWork;

    public BecomeHostRequestApprovedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BecomeHostRequestApproved userCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.UserRepository.GetUserByIdAsync(userCreatedEvent.BecomeHostRequest.UserId) is not User user)
        {
            throw new InvalidOperationException($"User Not found in database. UserId: {userCreatedEvent.BecomeHostRequest.UserId}");
        }

        var host = Host.Create(user.FirstName, user.LastName, user.Id);
        await _unitOfWork.HostRepository.AddAsync(host);
    }
}