using BuberDinner.Domain.Common.Models;

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
}