using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.SharedKernel;

using ErrorOr;

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

    public static ErrorOr<GuestId> Create(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            return Errors.Guest.InvalidGuestId;
        }

        return new GuestId(guid);
    }
}