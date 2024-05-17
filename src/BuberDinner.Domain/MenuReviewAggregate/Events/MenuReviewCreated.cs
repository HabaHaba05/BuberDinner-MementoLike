using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuReviewAggregate.Events;
public record MenuReviewCreated(MenuReview MenuReview) : IDomainEvent;