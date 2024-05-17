using FluentValidation;

namespace BuberDinner.Application.BecomeHost.Commands.UpdateStatus;

public class UpdateBecomeHostRequestStatusCommandValidator : AbstractValidator<UpdateBecomeHostRequestStatusCommand>
{
    public UpdateBecomeHostRequestStatusCommandValidator()
    {
        RuleFor(x => x.BecomeHostRequestId).NotEmpty();
    }
}