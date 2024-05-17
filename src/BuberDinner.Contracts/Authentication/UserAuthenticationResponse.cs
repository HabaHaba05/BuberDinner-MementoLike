namespace BuberDinner.Contracts.Authentication;

public record UserAuthenticationResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Token);