using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.UserAggregate.Events;
public record UserCreated(User User) : IDomainEvent;