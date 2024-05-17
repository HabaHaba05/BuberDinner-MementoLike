using BuberDinner.Domain.Common.ValueObjects;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.Domain.MenuReviewAggregate.Events;
using BuberDinner.Domain.MenuReviewAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuReviewAggregate;

public sealed class MenuReview : AggregateRoot<MenuReviewId, Guid>
{
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }
    public HostId HostId { get; private set; }
    public MenuId MenuId { get; private set; }
    public GuestId GuestId { get; private set; }
    public DinnerId DinnerId { get; private set; }

    private MenuReview(
        MenuReviewId menuReviewId,
        Rating rating,
        string comment,
        HostId hostId,
        MenuId menuId,
        GuestId guestId,
        DinnerId dinnerId)
        : base(menuReviewId)
    {
        Rating = rating;
        Comment = comment;
        HostId = hostId;
        MenuId = menuId;
        GuestId = guestId;
        DinnerId = dinnerId;
    }

    public static MenuReview Create(
        int rating,
        string comment,
        HostId hostId,
        MenuId menuId,
        GuestId guestId,
        DinnerId dinnerId)
    {
        // TODO: enforce invariants
        var ratingValueObject = Rating.Create(rating);
        var menuReview = new MenuReview(
            MenuReviewId.CreateUnique(),
            ratingValueObject,
            comment,
            hostId,
            menuId,
            guestId,
            dinnerId);
        menuReview.AddDomainEvent(new MenuReviewCreated(menuReview));
        return menuReview;
    }

    public static string TableName => "MenuReviews";
    public static MenuReview FromState(Dictionary<string, object?> state) => new(
        MenuReviewId.Create((Guid)state["Id"]!),
        Rating.Create((int)state["Rating_Value"]!),
        (string)state["Comment"]!,
        HostId.Create((Guid)state["HostId"]!),
        MenuId.Create((Guid)state["MenuId"]!),
        GuestId.Create((Guid)state["Id"]!),
        DinnerId.Create((Guid)state["DinnerId"]!));

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "Rating_Value", Rating.Value },
        { "Comment", Comment },
        { "HostId", HostId.Value },
        { "MenuId", MenuId.Value },
        { "GuestId", GuestId.Value },
        { "DinnerId", DinnerId.Value },
    };
}