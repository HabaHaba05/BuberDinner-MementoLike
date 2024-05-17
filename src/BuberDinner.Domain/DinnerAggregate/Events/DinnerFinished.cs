using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.DinnerAggregate.Events;

public record DinnerFinished(Dinner Dinner) : IDomainEvent;