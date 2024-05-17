using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.SharedKernel;

using ErrorOr;

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

    public static ErrorOr<ReservationId> Create(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            return Errors.Reservation.InvalidReservationId;
        }

        return new ReservationId(guid);
    }
}