using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate.Enums;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.DinnerAggregate.Entities;

public sealed class Reservation : Entity<ReservationId>
{
    private readonly DinnerId _dinnerId;
    public int GuestCount { get; private set; }
    public GuestId GuestId { get; private set; }
    public BillId? BillId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime? ArrivalDateTime { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime? UpdatedDateTime { get; private set; }

    private Reservation(
        DinnerId dinnerId,
        GuestId guestId,
        int guestCount,
        DateTime? arrivalDateTime,
        BillId? billId,
        ReservationStatus status,
        ReservationId id)
        : base(id)
    {
        _dinnerId = dinnerId;
        GuestId = guestId;
        GuestCount = guestCount;
        ArrivalDateTime = arrivalDateTime;
        BillId = billId;
        Status = status;
        CreatedDateTime = DateTime.UtcNow;
    }

    public static ErrorOr<Reservation> Create(
        DinnerId dinnerId,
        GuestId guestId,
        int guestCount,
        ReservationStatus status,
        BillId? billId = null,
        DateTime? arrivalDateTime = null)
    {
        if (guestCount == 0)
        {
            return Errors.Reservation.GuestCountCannotBeLessThanOne;
        }

        var reservation = new Reservation(
            dinnerId,
            guestId,
            guestCount,
            arrivalDateTime,
            billId,
            status,
            ReservationId.CreateUnique());

        reservation.AddDomainEvent(new ReservationCreated(reservation));
        return reservation;
    }

    public Error? Cancel(GuestId cancelledByGuestId)
    {
        if (cancelledByGuestId != GuestId)
        {
            return Errors.Reservation.ReservationDoesNotBelongToGuest;
        }

        UpdatedDateTime = DateTime.UtcNow;
        Status = ReservationStatus.Cancelled;
        AddDomainEvent(new ReservationCancelled(this));
        return null;
    }

    public Error? Accept(GuestId acceptedByGuestId)
    {
        if (acceptedByGuestId != GuestId)
        {
            return Errors.Reservation.ReservationDoesNotBelongToGuest;
        }

        UpdatedDateTime = DateTime.UtcNow;
        Status = ReservationStatus.Reserved;
        AddDomainEvent(new ReservationAccepted(this));

        return null;
    }

    public Error? Reject(GuestId acceptedByGuestId)
    {
        if (acceptedByGuestId != GuestId)
        {
            return Errors.Reservation.ReservationDoesNotBelongToGuest;
        }

        UpdatedDateTime = DateTime.UtcNow;
        Status = ReservationStatus.Rejected;
        AddDomainEvent(new ReservationRejected(this));

        return null;
    }

    public void NoAnswer()
    {
        UpdatedDateTime = DateTime.UtcNow;
        Status = ReservationStatus.NoAnswer;
        AddDomainEvent(new GuestHasNotAnsweredToInvitation(this));
    }

    public void GuestArrived()
    {
        ArrivalDateTime = UpdatedDateTime = DateTime.UtcNow;
        Status = ReservationStatus.Arrived;
        AddDomainEvent(new GuestArrivedToDinner(GuestId, _dinnerId));
    }

    public void Complete()
    {
        if (Status == ReservationStatus.Reserved)
        {
            Status = ReservationStatus.NonArrival;
            UpdatedDateTime = DateTime.UtcNow;
            AddDomainEvent(new GuestHasNotArrivedToDinner(this));
        }
        else if (Status == ReservationStatus.Arrived)
        {
            Status = ReservationStatus.Completed;
            UpdatedDateTime = DateTime.UtcNow;
            AddDomainEvent(new ReservationCompleted(this));
        }
    }

    public void AddBill(BillId billId)
    {
        BillId = billId;
    }

    public static Reservation FromState(Dictionary<string, object?> state) => new(
        DinnerId.Create((Guid)state["DinnerId"]!),
        GuestId.Create((Guid)state["GuestId"]!),
        (int)state["GuestCount"]!,
        (DateTime?)state["ArrivalDateTime"],
        state["BillId"] is string ? BillId.Create((string)state["BillId"]!) : null,
        ReservationStatus.FromValue((int)state["Status"]!),
        ReservationId.Create((Guid)state["ReservationId"]!))
    {
        CreatedDateTime = (DateTime)state["CreatedDateTime"]!,
        UpdatedDateTime = (DateTime?)state["UpdatedDateTime"]!,
    };

    public Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "ReservationId" } },
        { "ReservationId", Id.Value },
        { "DinnerId", _dinnerId.Value },
        { "GuestCount", GuestCount },
        { "GuestId", GuestId.Value },
        { "BillId", BillId?.Value },
        { "Status", Status.Value },
        { "ArrivalDateTime", ArrivalDateTime },
        { "CreatedDateTime", CreatedDateTime },
        { "UpdatedDateTime", UpdatedDateTime},
    };

#pragma warning disable CS8618
    private Reservation()
    {
    }
#pragma warning restore CS8618
}