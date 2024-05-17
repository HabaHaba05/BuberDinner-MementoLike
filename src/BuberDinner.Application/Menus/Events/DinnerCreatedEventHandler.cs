using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;

using MediatR;

namespace BuberDinner.Application.Menus.Events;

public class DinnerCreatedEventHandler : INotificationHandler<DinnerCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public DinnerCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DinnerCreated dinnerCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.MenuRepository.GetByIdAsync(dinnerCreatedEvent.Dinner.MenuId) is not Menu menu)
        {
            throw new InvalidOperationException($"Dinner has invalid menu id (dinner id: {dinnerCreatedEvent.Dinner.Id}, menu id: {dinnerCreatedEvent.Dinner.MenuId}).");
        }

        menu.AddDinnerId((DinnerId)dinnerCreatedEvent.Dinner.Id);

        await _unitOfWork.MenuRepository.UpdateAsync(menu);
    }
}