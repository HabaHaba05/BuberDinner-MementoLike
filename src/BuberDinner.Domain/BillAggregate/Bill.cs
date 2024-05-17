using BuberDinner.Domain.BillAggregate.Enums;
using BuberDinner.Domain.BillAggregate.Events;
using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.BillAggregate;

public sealed class Bill : AggregateRoot<BillId, string>
{
    public DinnerId DinnerId { get; private set; }
    public GuestId GuestId { get; private set; }
    public HostId HostId { get; private set; }
    public Price Amount { get; private set; }
    public BillStatus Status { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Bill(
        DinnerId dinnerId,
        GuestId guestId,
        HostId hostId,
        Price amount,
        BillStatus billStatus,
        DateTime createdDateTime)
        : base(BillId.Create(dinnerId, guestId))
    {
        Status = billStatus;
        DinnerId = dinnerId;
        GuestId = guestId;
        HostId = hostId;
        Amount = amount;
        CreatedDateTime = createdDateTime;
    }

    public static Bill Create(
        DinnerId dinnerId,
        GuestId guestId,
        HostId hostId,
        Price amount)
    {
        // TODO: enforce invariants
        var bill = new Bill(
            dinnerId,
            guestId,
            hostId,
            amount,
            BillStatus.Unpaid,
            DateTime.UtcNow);

        bill.AddDomainEvent(new BillCreated(bill));

        return bill;
    }

    public void Pay()
    {
        Status = BillStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public static string TableName => "Bills";
    public static Bill FromState(Dictionary<string, object?> state) => new(
                    DinnerId.Create((Guid)state["DinnerId"]!),
                    GuestId.Create((Guid)state["GuestId"]!),
                    HostId.Create((Guid)state["HostId"]!),
                    Price.Create((decimal)state["Amount_Amount"]!, (string)state["Amount_Currency"]!),
                    BillStatus.FromValue((int)state["Status"]!),
                    (DateTime)state["CreatedDateTime"]!)
    {
        Id = BillId.Create((string)state["Id"]!),
        PaidAt = (DateTime?)state["PaidAt"]!,
    };

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "DinnerId", DinnerId.Value },
        { "GuestId", GuestId.Value },
        { "HostId", HostId.Value },
        { "Amount_Amount", Amount.Amount },
        { "Amount_Currency", Amount.Currency },
        { "Status", Status.Value },
        { "PaidAt", PaidAt },
        { "CreatedDateTime", CreatedDateTime },
    };
}