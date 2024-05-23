using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.DinnerAggregate.ValueObjects;

public sealed class ReservationId : EntityId<Guid>
{
    private ReservationId(Guid value) : base(value)
    {
    }

    public static ReservationId CreateUnique()
    {
        return new ReservationId(Guid.NewGuid());
    }

    public static ReservationId Create(Guid value)
    {
        return new ReservationId(value);
    }
}