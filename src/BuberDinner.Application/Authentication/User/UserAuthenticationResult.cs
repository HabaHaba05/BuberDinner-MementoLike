namespace BuberDinner.Application.Authentication.User;

public record UserAuthenticationResult(
    Domain.UserAggregate.User User,
    string Token);