using BuberDinner.Domain.Common.ValueObjects;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.GuestAggregate.Entities;

public sealed class GuestRating : Entity<GuestRatingId>
{
    private readonly GuestId _guestId;
    public HostId HostId { get; private set; }
    public DinnerId DinnerId { get; private set; }
    public Rating Rating { get; private set; }
    public DateTime CreatedDateTime { get; private set; }

    private GuestRating(DinnerId dinnerId, HostId hostId, Rating rating, GuestRatingId id, GuestId guestId)
        : base(id)
    {
        DinnerId = dinnerId;
        HostId = hostId;
        Rating = rating;
        CreatedDateTime = DateTime.UtcNow;
        _guestId = guestId;
    }

    public static GuestRating Create(DinnerId dinnerId, HostId hostId, int rating, GuestId guestId)
    {
        var ratingValueObject = Rating.Create(rating);
        return new GuestRating(dinnerId, hostId, ratingValueObject, GuestRatingId.CreateUnique(), guestId);
    }

    public static GuestRating FromState(Dictionary<string, object?> state)
    {
        var obj = new GuestRating(
            DinnerId.Create((Guid)state["DinnerId"]!),
            HostId.Create((Guid)state["HostId"]!),
            Rating.Create((int)state["Rating_Value"]!),
            GuestRatingId.Create((Guid)state["GuestRatingId"]!),
            GuestId.Create((Guid)state["GuestId"]!))
        {
            CreatedDateTime = DateTime.Parse(state["CreatedDateTime"]?.ToString()!),
        };
        return obj;
    }

    public Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "GuestRatingId" } },
        { "GuestRatingId", Id.Value },
        { "HostId", HostId.Value },
        { "DinnerId", DinnerId.Value },
        { "Rating_Value", Rating.Value },
        { "CreatedDateTime", CreatedDateTime },
        { "GuestId", _guestId.Value },
    };
}