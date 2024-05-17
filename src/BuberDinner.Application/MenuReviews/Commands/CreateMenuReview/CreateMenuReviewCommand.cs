using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.MenuReviewAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.MenuReviews.Commands.CreateMenuReview;
public record CreateMenuReviewCommand(GuestId GuestId, string DinnerId, string Comment, int Rating)
    : IRequest<ErrorOr<MenuReview>>;