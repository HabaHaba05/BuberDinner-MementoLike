using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.DinnerAggregate.Events;

public record DinnerCreated(Dinner Dinner) : IDomainEvent;