using BuberDinner.Application.Authentication.Admin;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Authentication.Admin.Queries.Login;

public record AdminLoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AdminAuthenticationResult>>;