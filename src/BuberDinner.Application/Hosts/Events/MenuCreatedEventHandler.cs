using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate;
using BuberDinner.Domain.MenuAggregate.Events;

using MediatR;

namespace BuberDinner.Application.Hosts.Events;

public class MenuCreatedEventHandler : INotificationHandler<MenuCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public MenuCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MenuCreated menuCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.HostRepository.GetByIdAsync(menuCreatedEvent.Menu.HostId) is not Host host)
        {
            throw new InvalidOperationException($"Host Not found in database. HostId: {menuCreatedEvent.Menu.HostId}");
        }

        host.AddMenu(menuCreatedEvent.Menu.Id);
        await _unitOfWork.HostRepository.UpdateAsync(host);
    }
}