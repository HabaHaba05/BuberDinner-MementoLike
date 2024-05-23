using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.GuestAggregate.Entities;

namespace BuberDinner.Domain.GuestAggregate.ValueObjects;

public sealed class GuestRatingId : EntityId<Guid>
{
    private GuestRatingId(Guid value) : base(value)
    {
    }

    public static GuestRatingId CreateUnique()
    {
        return new GuestRatingId(Guid.NewGuid());
    }

    public static GuestRatingId Create(Guid value)
    {
        return new GuestRatingId(value);
    }
}