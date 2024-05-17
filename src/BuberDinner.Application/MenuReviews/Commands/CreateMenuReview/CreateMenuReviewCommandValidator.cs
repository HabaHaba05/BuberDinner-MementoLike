using FluentValidation;

namespace BuberDinner.Application.MenuReviews.Commands.CreateMenuReview;

public class CreateMenuReviewCommandValidator : AbstractValidator<CreateMenuReviewCommand>
{
    public CreateMenuReviewCommandValidator()
    {
        RuleFor(x => x.Comment).NotEmpty();
        RuleFor(x => x.DinnerId).NotNull().NotEmpty();
        RuleFor(x => x.GuestId).NotNull().NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}