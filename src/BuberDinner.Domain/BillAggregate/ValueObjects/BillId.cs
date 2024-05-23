using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

namespace BuberDinner.Domain.BillAggregate.ValueObjects;

public sealed class BillId : AggregateRootId<string>
{
    private BillId(string value) : base(value)
    {
    }

    private BillId(DinnerId dinnerId, GuestId guestId)
        : base($"Bill_{dinnerId.Value}_{guestId.Value}")
    {
    }

    public static BillId Create(DinnerId dinnerId, GuestId guestId)
    {
        return new BillId(dinnerId, guestId);
    }

    public static BillId Create(string value)
    {
        return new BillId(value);
    }
}