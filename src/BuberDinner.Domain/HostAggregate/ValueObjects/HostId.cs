using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.UserAggregate.ValueObjects;

namespace BuberDinner.Domain.HostAggregate.ValueObjects;

public sealed class HostId : AggregateRootId<string>
{
    private HostId(string value) : base(value)
    {
    }

    public static HostId Create(UserId userId)
    {
        // TODO: enforce invariants
        return new HostId($"Host_{userId.Value}");
    }

    public static HostId Create(string hostId)
    {
        // TODO: enforce invariants
        return new HostId(hostId);
    }
}