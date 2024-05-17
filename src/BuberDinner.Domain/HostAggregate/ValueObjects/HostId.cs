using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.HostAggregate.ValueObjects;

public sealed class HostId : AggregateRootId<Guid>
{
    private HostId(Guid value) : base(value)
    {
    }

    public static HostId CreateUnique()
    {
        return new HostId(Guid.NewGuid());
    }

    public static HostId Create(Guid userId)
    {
        return new HostId(userId);
    }

    public static ErrorOr<HostId> Create(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            return Errors.Host.InvalidHostId;
        }

        return new HostId(guid);
    }
}