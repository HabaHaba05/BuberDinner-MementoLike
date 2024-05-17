using BuberDinner.SharedKernel;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

namespace BuberDinner.Domain.DinnerAggregate.Events;
public record GuestHasNotArrivedToDinner(Reservation Reservation) : IDomainEvent;