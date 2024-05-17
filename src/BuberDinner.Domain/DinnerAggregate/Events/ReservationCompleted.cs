using BuberDinner.SharedKernel;
using BuberDinner.Domain.DinnerAggregate.Entities;

namespace BuberDinner.Domain.DinnerAggregate.Events;
public record ReservationCompleted(Reservation Reservation) : IDomainEvent;