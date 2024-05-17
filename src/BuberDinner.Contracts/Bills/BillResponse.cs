using BuberDinner.Contracts.Dinners;

namespace BuberDinner.Contracts.Bills;
public record BillResponse(
    string DinnerId,
    string GuestId,
    string HostId,
    Price Price,
    BillStatus Status,
    DateTime CreatedDateTime,
    DateTime PaidAt);

public enum BillStatus
{
    Unpaid,
    Paid,
}