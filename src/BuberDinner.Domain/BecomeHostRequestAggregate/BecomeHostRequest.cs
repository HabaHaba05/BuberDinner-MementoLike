using Ardalis.GuardClauses;

using BuberDinner.Domain.AdminAggregate.ValueObjects;
using BuberDinner.Domain.BecomeHostRequestAggregate.Enums;
using BuberDinner.Domain.BecomeHostRequestAggregate.Events;
using BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.BecomeHostRequestAggregate;

public sealed class BecomeHostRequest : AggregateRoot<BecomeHostRequestId, Guid>
{
    public UserId UserId { get; private set; }
    public AdminId? ReviewedByAdminId { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public RequestResponseType Status { get; private set; }

    private BecomeHostRequest(UserId userId, BecomeHostRequestId becomeHostRequestId)
        : base(becomeHostRequestId)
    {
        UserId = userId;
        Status = RequestResponseType.Pending;
        CreatedDateTime = DateTime.UtcNow;
    }

    public static BecomeHostRequest Create(UserId userId)
    {
        Guard.Against.Null(userId);
        return new BecomeHostRequest(userId, BecomeHostRequestId.CreateUnique());
    }

    public Error? Approve(AdminId approvedByAdmin)
    {
        if (!CanBeUpdated())
        {
            return Errors.BecomeHostRequest.StatusUpdateNotAllowed;
        }

        ReviewedByAdminId = approvedByAdmin;
        Status = RequestResponseType.Approved;
        ReviewedAt = DateTime.UtcNow;
        AddDomainEvent(new BecomeHostRequestApproved(this));

        return null;
    }

    public Error? Reject(AdminId rejectedByAdmin)
    {
        if (!CanBeUpdated())
        {
            return Errors.BecomeHostRequest.StatusUpdateNotAllowed;
        }

        ReviewedByAdminId = rejectedByAdmin;
        Status = RequestResponseType.Rejected;
        ReviewedAt = DateTime.UtcNow;
        AddDomainEvent(new BecomeHostRequestRejected(this));
        return null;
    }

    public static string TableName => "BecomeHostRequests";

    public static BecomeHostRequest FromState(Dictionary<string, object?> state)
    {
        AdminId? reviewedByAdminId = state["ReviewedByAdminId"] is not null
            ? AdminId.Create((Guid)state["ReviewedByAdminId"]!)
            : null;

        var obj = new BecomeHostRequest(
            UserId.Create((Guid)state["UserId"]!),
            BecomeHostRequestId.Create((Guid)state["Id"]!))
        {
            CreatedDateTime = (DateTime)state["CreatedDateTime"]!,
            Status = RequestResponseType.FromValue((int)state["Status"]!),
            ReviewedAt = (DateTime?)state["ReviewedAt"],
            ReviewedByAdminId = reviewedByAdminId,
        };
        return obj;
    }

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "UserId", UserId.Value },
        { "ReviewedByAdminId", ReviewedByAdminId?.Value },
        { "CreatedDateTime", CreatedDateTime },
        { "ReviewedAt", ReviewedAt },
        { "Status", Status.Value },
    };

    private bool CanBeUpdated() => Status == RequestResponseType.Pending;
}