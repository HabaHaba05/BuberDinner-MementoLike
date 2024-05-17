using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.DinnerAggregate.Events;

public record DinnerCreated(Dinner Dinner) : IDomainEvent;