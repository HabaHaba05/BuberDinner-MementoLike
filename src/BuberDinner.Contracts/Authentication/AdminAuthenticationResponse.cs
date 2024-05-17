namespace BuberDinner.Contracts.Authentication;

public record AdminAuthenticationResponse(
    string Id,
    string Name,
    string Email,
    string Token);