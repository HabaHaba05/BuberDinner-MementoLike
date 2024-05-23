using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.GuestAggregate.ValueObjects;

public sealed class GuestId : AggregateRootId<Guid>
{
    private GuestId(Guid value) : base(value)
    {
    }

    public static GuestId CreateUnique()
    {
        return new GuestId(Guid.NewGuid());
    }

    public static GuestId Create(Guid value)
    {
        return new GuestId(value);
    }
}