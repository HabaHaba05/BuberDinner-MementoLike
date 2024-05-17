using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;

public sealed class BecomeHostRequestId : AggregateRootId<Guid>
{
    private BecomeHostRequestId(Guid value) : base(value)
    {
    }

    public static BecomeHostRequestId CreateUnique()
    {
        return new BecomeHostRequestId(Guid.NewGuid());
    }

    public static BecomeHostRequestId Create(Guid userId)
    {
        return new BecomeHostRequestId(userId);
    }

    public static ErrorOr<BecomeHostRequestId> Create(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            return Errors.BecomeHostRequest.InvalidBecomeHostRequestId;
        }

        return new BecomeHostRequestId(guid);
    }
}