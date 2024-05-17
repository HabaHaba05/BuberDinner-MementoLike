using FluentValidation;

namespace BuberDinner.Application.Authentication.Admin.Queries.Login;

public class AdminLoginQueryValidator : AbstractValidator<AdminLoginQuery>
{
    public AdminLoginQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}