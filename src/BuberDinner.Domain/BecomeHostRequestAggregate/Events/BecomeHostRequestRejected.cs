
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.BecomeHostRequestAggregate.Events;

public record BecomeHostRequestRejected(BecomeHostRequest BecomeHostRequest) : IDomainEvent;