using FluentValidation;

namespace BuberDinner.Application.Dinners.Commands.CreateDinnerReservation;

public class CreateDinnerReservationCommandValidator : AbstractValidator<CreateDinnerReservationCommand>
{
    public CreateDinnerReservationCommandValidator()
    {
        RuleFor(x => x.GuestsCount).GreaterThan(0);
        RuleFor(x => x.DinnerId).NotEmpty();
        RuleFor(x => x.GuestId).NotEmpty();
    }
}