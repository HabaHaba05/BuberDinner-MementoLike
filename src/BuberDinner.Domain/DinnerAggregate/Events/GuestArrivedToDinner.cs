using BuberDinner.SharedKernel;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

namespace BuberDinner.Domain.DinnerAggregate.Events;
public record GuestArrivedToDinner(GuestId GuestId, DinnerId DinnerId) : IDomainEvent;