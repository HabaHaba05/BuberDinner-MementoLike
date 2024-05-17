namespace BuberDinner.Contracts.Guests;
public record CreateGuestRatingRequest(string DinnerId, string GuestId, int Rating);