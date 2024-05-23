using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Menus.Events;

public class DinnerCreatedEventHandler : INotificationHandler<DinnerCreated>
{
    private readonly IMenuRepository _menuRepository;

    public DinnerCreatedEventHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task Handle(DinnerCreated dinnerCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _menuRepository.GetByIdAsync(dinnerCreatedEvent.Dinner.MenuId) is not Menu menu)
        {
            throw new InvalidOperationException($"Dinner has invalid menu id (dinner id: {dinnerCreatedEvent.Dinner.Id}, menu id: {dinnerCreatedEvent.Dinner.MenuId}).");
        }

        menu.AddDinnerId((DinnerId)dinnerCreatedEvent.Dinner.Id);

        await _menuRepository.UpdateAsync(menu);
    }
}