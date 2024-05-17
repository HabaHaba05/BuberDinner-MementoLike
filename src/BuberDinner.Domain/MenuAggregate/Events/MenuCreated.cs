using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuAggregate.Events;

public record MenuCreated(Menu Menu) : IDomainEvent;