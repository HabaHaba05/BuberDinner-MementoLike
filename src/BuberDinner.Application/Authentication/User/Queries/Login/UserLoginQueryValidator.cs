using FluentValidation;

namespace BuberDinner.Application.Authentication.User.Queries.Login;

public class UserLoginQueryValidator : AbstractValidator<UserLoginQuery>
{
    public UserLoginQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}