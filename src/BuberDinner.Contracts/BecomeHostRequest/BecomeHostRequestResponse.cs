namespace BuberDinner.Contracts.BecomeHostRequest;
public record BecomeHostRequestResponse(
    string Id,
    string UserId,
    string Status,
    DateTime CreatedAt,
    string? ReviewedByAdminId,
    DateTime? ReviewedAt);