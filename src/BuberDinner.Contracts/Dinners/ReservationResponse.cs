namespace BuberDinner.Contracts.Dinners;
public record ReservationResponse(
    string ReservationId,
    int GuestCount,
    string GuestId,
    string? BillId,
    ReservationStatus Status,
    DateTime? ArrivalDateTime,
    DateTime CreatedDateTime,
    DateTime UpdatedDateTime);

public enum ReservationStatus
{
    PendingGuestApproval,
    Rejected,
    NoAnswer,
    Reserved,
    Cancelled,
    Arrived,
    Completed,
    NonArrival,
}