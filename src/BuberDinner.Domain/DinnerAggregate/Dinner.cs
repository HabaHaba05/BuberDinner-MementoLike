using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.DinnerAggregate.Enums;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.SharedKernel;

using ErrorOr;

namespace BuberDinner.Domain.DinnerAggregate;

public sealed class Dinner : AggregateRoot<DinnerId, Guid>
{
    private List<Reservation> _reservations = new();
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public DateTime? StartedDateTime { get; private set; }
    public DateTime? EndedDateTime { get; private set; }
    public DinnerStatus Status { get; private set; }
    public bool IsPublic { get; private set; }
    public int MaxGuests { get; private set; }
    public Price Price { get; private set; }
    public HostId HostId { get; private set; }
    public MenuId MenuId { get; private set; }
    public Location Location { get; private set; }

    public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();
    public IReadOnlyList<Reservation> ArrivedReservations => _reservations.Where(x => x.Status == ReservationStatus.Arrived).ToList().AsReadOnly();
    public IReadOnlyList<Reservation> CompletedReservations => _reservations.Where(x => x.Status == ReservationStatus.Completed).ToList().AsReadOnly();

    public DateTime CreatedDateTime { get; private set; }

    private Dinner(
        DinnerId dinnerId,
        string name,
        string description,
        DateTime startDateTime,
        DateTime endDateTime,
        bool isPublic,
        int maxGuests,
        Price price,
        MenuId menuId,
        HostId hostId,
        Location location)
        : base(dinnerId)
    {
        Name = name;
        Description = description;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        IsPublic = isPublic;
        MaxGuests = maxGuests;
        Price = price;
        MenuId = menuId;
        HostId = hostId;
        Location = location;
        Status = DinnerStatus.Upcoming;
        CreatedDateTime = DateTime.UtcNow;
    }

    public static Dinner Create(
        string name,
        string description,
        DateTime startDateTime,
        DateTime endDateTime,
        bool isPublic,
        int maxGuests,
        Price price,
        MenuId menuId,
        HostId hostId,
        Location location)
    {
        // enforce invariants
        var dinner = new Dinner(
            DinnerId.CreateUnique(),
            name,
            description,
            startDateTime,
            endDateTime,
            isPublic,
            maxGuests,
            price,
            menuId,
            hostId,
            location);

        dinner.AddDomainEvent(new DinnerCreated(dinner));

        return dinner;
    }

    public static string TableName => "Dinners";
    public static Dinner FromState(Dictionary<string, object?> state) => new(
            DinnerId.Create((Guid)state["Id"]!),
            (string)state["Name"]!,
            (string)state["Description"]!,
            (DateTime)state["StartDateTime"]!,
            (DateTime)state["EndDateTime"]!,
            (bool)state["IsPublic"]!,
            (int)state["MaxGuests"]!,
            Price.Create((decimal)state["Price_Amount"]!, (string)state["Price_Currency"]!),
            MenuId.Create((Guid)state["MenuId"]!),
            HostId.Create((Guid)state["HostId"]!),
            Location.Create(
                (string)state["Location_Name"]!,
                (string)state["Location_Address"]!,
                (double)state["Location_Latitude"]!,
                (double)state["Location_Longitude"]!))
    {
        CreatedDateTime = (DateTime)state["CreatedDateTime"]!,
        Status = DinnerStatus.FromValue((int)state["Status"]!),
        _reservations = ((IEnumerable<IDictionary<string, object?>>)state["DinnerReservations"]!).Select(x => Reservation.FromState(x.ToDictionary())).ToList(),
    };

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "Name", Name },
        { "Description", Description},
        { "StartDateTime", StartDateTime },
        { "EndDateTime", EndDateTime },
        { "StartedDateTime", StartedDateTime },
        { "EndedDateTime", EndedDateTime },
        { "Status", Status.Value },
        { "IsPublic", IsPublic },
        { "MaxGuests", MaxGuests },
        { "Price_Amount", Price.Amount },
        { "Price_Currency", Price.Currency },
        { "HostId", HostId.Value },
        { "MenuId", MenuId.Value },
        { "Location_Address", Location.Address },
        { "Location_Name", Location.Name },
        { "Location_Latitude", Location.Latitude },
        { "Location_Longitude", Location.Longitude },
        { "DinnerReservations", _reservations.Select(x => x.GetState()) },
        { "CreatedDateTime", CreatedDateTime },
    };

    public ErrorOr<Reservation> InviteGuest(GuestId guestId, HostId hostId) =>
        HostId != hostId
            ? Errors.Dinner.DinnerBelongsToDifferentHost
            : CreateNewReservationRequest(guestId, 1, ReservationStatus.PendingGuestApproval);
    public ErrorOr<Reservation> AddReservation(GuestId guestId, int guestCount) => CreateNewReservationRequest(guestId, guestCount, ReservationStatus.Reserved);

    public ErrorOr<Reservation> CancelReservation(ReservationId reservationId, GuestId guestId)
    {
        if (Status == DinnerStatus.Upcoming)
        {
            if (PendingReservations.SingleOrDefault(x => x.Id == reservationId) is not Reservation reservationToBeCancelled)
            {
                return Errors.Dinner.DinnerDoesNotContainProvidedReservation;
            }

            var cancelReservationResult = reservationToBeCancelled.Cancel(guestId);
            if (cancelReservationResult.HasValue)
            {
                return cancelReservationResult.Value;
            }

            return reservationToBeCancelled;
        }

        return Errors.Dinner.ReservationsCannotBeUpdated;
    }

    public ErrorOr<Dinner> GuestArrived(HostId updatedByHostId, GuestId guestId)
    {
        if (HostId != updatedByHostId)
        {
            return Errors.Dinner.DinnerBelongsToDifferentHost;
        }

        var guestReservation = _reservations
            .Where(x => x.Status == ReservationStatus.Reserved && x.GuestId == guestId)
            .Single();
        guestReservation.GuestArrived();
        return this;
    }

    public ErrorOr<Dinner> Start(HostId startedByHostId)
    {
        if (HostId != startedByHostId)
        {
            return Errors.Dinner.DinnerBelongsToDifferentHost;
        }

        StartedDateTime = DateTime.UtcNow;
        Status = DinnerStatus.InProgress;
        _reservations
            .Where(x => x.Status == ReservationStatus.PendingGuestApproval)
            .ToList()
            .ForEach(x => x.NoAnswer());

        AddDomainEvent(new DinnerStarted(this));
        return this;
    }

    public ErrorOr<Dinner> Finish(HostId finishedByHostId)
    {
        if (HostId != finishedByHostId)
        {
            return Errors.Dinner.DinnerBelongsToDifferentHost;
        }

        Status = DinnerStatus.Ended;
        EndedDateTime = DateTime.UtcNow;

        _reservations.ForEach(reservation => reservation.Complete());
        AddDomainEvent(new DinnerFinished(this));
        return this;
    }

    public ErrorOr<Reservation> AcceptReservationInvitation(ReservationId reservationId, GuestId guestId) =>
        UpdateReservation(reservationId, guestId, true);
    public ErrorOr<Reservation> RejectReservationInvitation(ReservationId reservationId, GuestId guestId) =>
        UpdateReservation(reservationId, guestId, false);

    private ErrorOr<Reservation> UpdateReservation(ReservationId reservationId, GuestId guestId, bool acceptReservation)
    {
        if (Status == DinnerStatus.Upcoming)
        {
            if (_reservations
                    .Where(x => x.Status == ReservationStatus.PendingGuestApproval)
                    .SingleOrDefault(x => x.Id == reservationId) is not Reservation reservation)
            {
                return Errors.Dinner.DinnerDoesNotContainProvidedReservation;
            }

            Error? errorFromReservation = acceptReservation ? reservation.Accept(guestId) : reservation.Reject(guestId);
            if (errorFromReservation.HasValue)
            {
                return errorFromReservation.Value;
            }

            return reservation;
        }

        return Errors.Dinner.ReservationsCannotBeUpdated;
    }

    private ErrorOr<Reservation> CreateNewReservationRequest(GuestId guestId, int guestCount, ReservationStatus reservationStatus)
    {
        if (Status != DinnerStatus.Upcoming)
        {
            return Errors.Dinner.ReservationsCannotBeUpdated;
        }

        if (PendingReservations.Sum(x => x.GuestCount) + guestCount > MaxGuests)
        {
            return Errors.Dinner.MaxCapacityReached;
        }

        if (PendingReservations.Any(x => x.GuestId == guestId))
        {
            return Errors.Dinner.GuestAlreadyHasReservationToThisDinner;
        }

        var reservation = Reservation.Create(Id, guestId, guestCount, reservationStatus);
        if (reservation.IsError)
        {
            return reservation.Errors;
        }

        _reservations.Add(reservation.Value);
        return reservation;
    }

    private List<Reservation> PendingReservations =>
        _reservations.Where(x =>
            x.Status == ReservationStatus.PendingGuestApproval ||
            x.Status == ReservationStatus.Reserved).ToList();
}