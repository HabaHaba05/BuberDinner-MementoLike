using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Authentication.User.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<UserAuthenticationResult>>;