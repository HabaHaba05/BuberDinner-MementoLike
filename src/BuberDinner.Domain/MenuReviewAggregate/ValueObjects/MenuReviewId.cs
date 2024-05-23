using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.MenuReviewAggregate.ValueObjects;

public sealed class MenuReviewId : AggregateRootId<Guid>
{
    private MenuReviewId(Guid value) : base(value)
    {
    }

    public static MenuReviewId CreateUnique()
    {
        // TODO: enforce invariants
        return new MenuReviewId(Guid.NewGuid());
    }

    public static MenuReviewId Create(Guid value)
    {
        // TODO: enforce invariants
        return new MenuReviewId(value);
    }
}