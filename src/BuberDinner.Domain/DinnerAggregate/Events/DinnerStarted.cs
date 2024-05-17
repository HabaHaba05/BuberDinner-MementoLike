using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.DinnerAggregate.Events;
public record DinnerStarted(Dinner Dinner) : IDomainEvent;