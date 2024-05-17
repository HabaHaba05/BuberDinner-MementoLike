namespace BuberDinner.Application.Authentication.Admin;

public record AdminAuthenticationResult(
    Domain.AdminAggregate.Admin Admin,
    string Token);