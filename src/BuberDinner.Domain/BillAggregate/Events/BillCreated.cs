using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.BillAggregate.Events;
public record BillCreated(Bill Bill) : IDomainEvent;