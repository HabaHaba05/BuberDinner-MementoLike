using Ardalis.GuardClauses;

using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Domain.Common.Guards;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuReviewAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.GuestAggregate;

public sealed class Guest : AggregateRoot<GuestId, Guid>
{
    private List<ReservationId> _pendingReservationIds = new();
    private List<ReservationId> _upcomingReservationIds = new();
    private List<ReservationId> _ongoingReservationIds = new();
    private List<ReservationId> _pastReservationIds = new();
    private List<BillId> _billIds = new();
    private List<MenuReviewId> _menuReviewIds = new();
    private List<GuestRating> _ratings = new();

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserId UserId { get; private set; }

    public IReadOnlyList<ReservationId> PendingReservationIds => _pendingReservationIds.AsReadOnly();
    public IReadOnlyList<ReservationId> UpcomingReservationIds => _upcomingReservationIds.AsReadOnly();
    public IReadOnlyList<ReservationId> OngoingReservationIds => _ongoingReservationIds.AsReadOnly();
    public IReadOnlyList<ReservationId> PastReservationIds => _pastReservationIds.AsReadOnly();
    public IReadOnlyList<BillId> BillIds => _billIds.AsReadOnly();
    public IReadOnlyList<MenuReviewId> MenuReviewIds => _menuReviewIds.AsReadOnly();
    public IReadOnlyList<GuestRating> Ratings => _ratings.AsReadOnly();
    public DateTime CreatedDateTime { get; private set; }

    private Guest(string firstName, string lastName, UserId userId, GuestId guestId)
        : base(guestId)
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
        CreatedDateTime = DateTime.UtcNow;
    }

    public static Guest Create(string firstName, string lastName, UserId userId)
    {
        Guard.Against.NullOrEmpty(firstName);
        Guard.Against.NullOrEmpty(lastName);
        Guard.Against.Null(userId);

        return new Guest(firstName, lastName, userId, GuestId.CreateUnique());
    }

    public void AddPendingReservation(ReservationId reservationId)
    {
        ValidateThatReservationIdDoesNotExistInAnyList(reservationId);
        _pendingReservationIds.Add(reservationId);
    }

    public void AddUpcomingReservation(ReservationId reservationId)
    {
        _pendingReservationIds.Remove(reservationId);
        ValidateThatReservationIdDoesNotExistInAnyList(reservationId);
        _upcomingReservationIds.Add(reservationId);
    }

    public void ChangeReservationToOngoing(ReservationId reservationId)
    {
        Guard.Against.Contains(_upcomingReservationIds, reservationId);
        _upcomingReservationIds.Remove(reservationId);
        ValidateThatReservationIdDoesNotExistInAnyList(reservationId);
        _ongoingReservationIds.Add(reservationId);
    }

    public void ChangeReservationToFinished(ReservationId reservationId)
    {
        _pendingReservationIds.Remove(reservationId);
        _upcomingReservationIds.Remove(reservationId);
        _ongoingReservationIds.Remove(reservationId);
        ValidateThatReservationIdDoesNotExistInAnyList(reservationId);
        _pastReservationIds.Add(reservationId);
    }

    public void AddRating(HostId hostId, DinnerId dinnerId, int rating)
    {
        var guestRating = GuestRating.Create(dinnerId, hostId, rating, Id);
        _ratings.Add(guestRating);
    }

    public void AddBill(BillId billId) => _billIds.Add(billId);
    public void AddMenuReview(MenuReviewId menuReviewId) => _menuReviewIds.Add(menuReviewId);

    public static string TableName => "Guests";
    public static Guest FromState(Dictionary<string, object?> state) => new(
            (string)state["FirstName"]!,
            (string)state["LastName"]!,
            UserId.Create((Guid)state["UserId"]!),
            GuestId.Create((Guid)state["Id"]!))
    {
        CreatedDateTime = (DateTime)state["CreatedDateTime"]!,
        _menuReviewIds = ((IEnumerable<string>)state["GuestMenuReviewIds"]!).Select(MenuReviewId.Create).ToList(),
        _billIds = ((IEnumerable<string>)state["GuestBillIds"]!).Select(BillId.Create).ToList(),
        _pendingReservationIds = ((IEnumerable<string>)state["GuestPendingReservationIds"]!).Select(x => ReservationId.Create(x).Value).ToList(),
        _upcomingReservationIds = ((IEnumerable<string>)state["GuestUpcomingReservationIds"]!).Select(x => ReservationId.Create(x).Value).ToList(),
        _ongoingReservationIds = ((IEnumerable<string>)state["GuestOngoingReservationIds"]!).Select(x => ReservationId.Create(x).Value).ToList(),
        _pastReservationIds = ((IEnumerable<string>)state["GuestPastReservationIds"]!).Select(x => ReservationId.Create(x).Value).ToList(),
        _ratings = ((IEnumerable<IDictionary<string, object?>>)state["GuestRatings"]!).Select(x => GuestRating.FromState(x.ToDictionary())).ToList(),
    };

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "FirstName", FirstName },
        { "LastName", LastName },
        { "UserId", UserId.Value },
        { "CreatedDateTime", CreatedDateTime },
        { "GuestMenuReviewIds", _menuReviewIds.Select(x => new { Keys = new[] { "MenuReviewId", "GuestId" }, MenuReviewId = x.Value, GuestId = Id.Value }) },
        { "GuestBillIds", _billIds.Select(x => new { Keys = new[] { "BillId", "GuestId" }, BillId = x.Value, GuestId = Id.Value }) },
        { "GuestPendingReservationIds", _pendingReservationIds.Select(x => new { Keys = new[] { "ReservationId", "GuestId" }, ReservationId = x.Value, GuestId = Id.Value }) },
        { "GuestUpcomingReservationIds", _upcomingReservationIds.Select(x => new { Keys = new[] { "ReservationId", "GuestId" }, ReservationId = x.Value, GuestId = Id.Value }) },
        { "GuestOngoingReservationIds", _ongoingReservationIds.Select(x => new { Keys = new[] { "ReservationId", "GuestId" }, ReservationId = x.Value, GuestId = Id.Value }) },
        { "GuestPastReservationIds", _pastReservationIds.Select(x => new { Keys = new[] { "ReservationId", "GuestId" }, ReservationId = x.Value, GuestId = Id.Value }) },
        { "GuestRatings", _ratings.Select(x => x.GetState()) },
    };

    private void ValidateThatReservationIdDoesNotExistInAnyList(ReservationId reservationId)
    {
        Guard.Against.DoesNotExist(_pendingReservationIds, reservationId);
        Guard.Against.DoesNotExist(_upcomingReservationIds, reservationId);
        Guard.Against.DoesNotExist(_ongoingReservationIds, reservationId);
        Guard.Against.DoesNotExist(_pastReservationIds, reservationId);
    }
}