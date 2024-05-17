using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.BecomeHostRequestAggregate.Events;

public record BecomeHostRequestApproved(BecomeHostRequest BecomeHostRequest) : IDomainEvent;