using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.AdminAggregate.ValueObjects;

public sealed class AdminId : AggregateRootId<Guid>
{
    private AdminId(Guid value) : base(value)
    {
    }

    public static AdminId CreateUnique()
    {
        return new AdminId(Guid.NewGuid());
    }

    public static AdminId Create(Guid userId)
    {
        return new AdminId(userId);
    }
}