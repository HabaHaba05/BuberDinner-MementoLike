using FluentValidation;

namespace BuberDinner.Application.Guests.Commands;

public class CreateGuestRatingCommandValidator : AbstractValidator<CreateGuestRatingCommand>
{
    public CreateGuestRatingCommandValidator()
    {
        RuleFor(x => x.HostId).NotNull();
        RuleFor(x => x.DinnerId).NotNull().NotEmpty();
        RuleFor(x => x.GuestId).NotNull().NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}