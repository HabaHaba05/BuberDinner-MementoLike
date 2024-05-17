using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.HostAggregate;

using MediatR;

namespace BuberDinner.Application.Hosts.Events;

public class DinnerCreatedEventHandler : INotificationHandler<DinnerCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public DinnerCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DinnerCreated dinnerCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.HostRepository.GetByIdAsync(dinnerCreatedEvent.Dinner.HostId) is not Host host)
        {
            throw new InvalidOperationException($"Host Not found in database. HostId: {dinnerCreatedEvent.Dinner.HostId}");
        }

        host.AddDinner(dinnerCreatedEvent.Dinner.Id);
        await _unitOfWork.HostRepository.UpdateAsync(host);
    }
}