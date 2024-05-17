using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Authentication.User.Queries.Login;

public record UserLoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<UserAuthenticationResult>>;