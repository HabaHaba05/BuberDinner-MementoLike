using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.DinnerAggregate.ValueObjects;

public sealed class DinnerId : AggregateRootId<Guid>
{
    private DinnerId(Guid value) : base(value)
    {
    }

    public static DinnerId CreateUnique()
    {
        return new DinnerId(Guid.NewGuid());
    }

    public static DinnerId Create(Guid value)
    {
        return new DinnerId(value);
    }

    public static ErrorOr<DinnerId> Create(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            return Errors.Dinner.InvalidDinnerId;
        }

        return new DinnerId(guid);
    }
}