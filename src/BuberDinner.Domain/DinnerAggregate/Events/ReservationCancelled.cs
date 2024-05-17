using BuberDinner.SharedKernel;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

namespace BuberDinner.Domain.DinnerAggregate.Events;
public record ReservationCancelled(Reservation Reservation) : IDomainEvent;