using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.UserAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class UserCreatedEventHandler : INotificationHandler<UserCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserCreated userCreatedEvent, CancellationToken cancellationToken)
    {
        var user = userCreatedEvent.User;
        var guest = Guest.Create(user.FirstName, user.LastName, user.Id);
        await _unitOfWork.GuestRepository.AddAsync(guest);
    }
}