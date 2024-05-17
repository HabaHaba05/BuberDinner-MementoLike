using BuberDinner.Domain.Common.ValueObjects;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate.Entities;
using BuberDinner.Domain.MenuAggregate.Events;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.Domain.MenuReviewAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuAggregate;

public sealed class Menu : AggregateRoot<MenuId, Guid>
{
    private readonly List<MenuSection> _sections = new();
    private List<DinnerId> _dinnerIds = new();
    private List<MenuReviewId> _menuReviewIds = new();

    public string Name { get; private set; }
    public string Description { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public IReadOnlyList<MenuSection> Sections => _sections.AsReadOnly();
    public HostId HostId { get; private set; }

    public IReadOnlyList<DinnerId> DinnerIds => _dinnerIds.AsReadOnly();
    public IReadOnlyList<MenuReviewId> MenuReviewIds => _menuReviewIds.AsReadOnly();

    public DateTime CreatedDateTime { get; private set; }

    private Menu(
        MenuId menuId,
        HostId hostId,
        string name,
        string description,
        AverageRating averageRating,
        List<MenuSection> sections)
        : base(menuId)
    {
        HostId = hostId;
        Name = name;
        Description = description;
        AverageRating = averageRating;
        _sections = sections;
        CreatedDateTime = DateTime.UtcNow;
    }

    public static Menu Create(
        MenuId menuId,
        HostId hostId,
        string name,
        string description,
        List<MenuSection>? sections = null)
    {
        // TODO: enforce invariants
        var menu = new Menu(
            menuId,
            hostId,
            name,
            description,
            AverageRating.CreateNew(),
            sections ?? new());

        menu.AddDomainEvent(new MenuCreated(menu));

        return menu;
    }

    public void AddDinnerId(DinnerId dinnerId)
    {
        _dinnerIds.Add(dinnerId);
    }

    public void AddReview(Rating rating, MenuReviewId menuReviewId)
    {
        _menuReviewIds.Add(menuReviewId);
        AverageRating.AddNewRating(rating);
    }

    public static string TableName => "Menus";
    public static Menu FromState(Dictionary<string, object?> state) => new(
            MenuId.Create((Guid)state["Id"]!),
            HostId.Create((Guid)state["HostId"]!),
            (string)state["Name"]!,
            (string)state["Description"]!,
            AverageRating.CreateNew((double)state["AverageRating_Value"]!, (int)state["AverageRating_NumRatings"]!),
            ((IEnumerable<IDictionary<string, object?>>)state["MenuSections"]!).Select(x => MenuSection.FromState(x.ToDictionary())).ToList())
    {
        CreatedDateTime = (DateTime)state["CreatedDateTime"]!,
        _dinnerIds = ((IEnumerable<Guid>)state["MenuDinnerIds"]!).Select(DinnerId.Create).ToList(),
        _menuReviewIds = ((IEnumerable<Guid>)state["MenuReviewIds"]!).Select(MenuReviewId.Create).ToList(),
    };

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "Name", Name },
        { "Description", Description },
        { "AverageRating_Value", AverageRating.Value },
        { "AverageRating_NumRatings", AverageRating.NumRatings },
        { "HostId", HostId.Value },
        { "CreatedDateTime", CreatedDateTime },
        { "MenuSections", _sections.Select(x => x.GetState()) },
        { "MenuDinnerIds", _dinnerIds.Select(x => new { Keys = new[] {"DinnerId", "MenuId" }, DinnerId = x.Value, MenuId = Id.Value }) },
        { "MenuReviewIds", _menuReviewIds.Select(x => new { Keys = new[] {"ReviewId", "MenuId" }, ReviewId = x.Value, MenuId = Id.Value }) },
    };
}