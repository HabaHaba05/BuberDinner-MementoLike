using BuberDinner.Domain.Common.ValueObjects;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.HostAggregate;

public sealed class Host : AggregateRoot<HostId, Guid>
{
    private List<MenuId> _menuIds = new();
    private List<DinnerId> _dinnerIds = new();

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public UserId UserId { get; private set; }
    public IReadOnlyList<MenuId> MenuIds => _menuIds.AsReadOnly();
    public IReadOnlyList<DinnerId> DinnerIds => _dinnerIds.AsReadOnly();
    public DateTime CreatedDateTime { get; private set; }

    private Host(
        HostId hostId,
        string firstName,
        string lastName,
        UserId userId,
        AverageRating averageRating,
        DateTime createdDateTime)
        : base(hostId ?? HostId.CreateUnique())
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
        AverageRating = averageRating;
        CreatedDateTime = createdDateTime;
    }

    public static Host Create(
        string firstName,
        string lastName,
        UserId userId)
    {
        // TODO: enforce invariants
        return new Host(
            HostId.CreateUnique(),
            firstName,
            lastName,
            userId,
            AverageRating.CreateNew(),
            DateTime.UtcNow);
    }

    public void AddMenu(MenuId menuId) => _menuIds.Add(menuId);
    public void AddDinner(DinnerId dinnerId) => _dinnerIds.Add(dinnerId);
    public void AddNewRating(Rating rating) => AverageRating.AddNewRating(rating);

    public static string TableName => "Hosts";
    public static Host FromState(Dictionary<string, object?> state) => new(
            HostId.Create((Guid)state["Id"]!),
            (string)state["FirstName"]!,
            (string)state["LastName"]!,
            UserId.Create((Guid)state["UserId"]!),
            AverageRating.CreateNew((double)state["AverageRating_Value"]!, (int)state["AverageRating_NumRatings"]!),
            (DateTime)state["CreatedDateTime"]!)
    {
        _menuIds = ((IList<Guid>)state["HostMenuIds"]!).Select(MenuId.Create).ToList(),
        _dinnerIds = ((IList<Guid>)state["HostDinnerIds"]!).Select(DinnerId.Create).ToList(),
    };

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value },
        { "FirstName", FirstName },
        { "LastName", LastName },
        { "AverageRating_Value", AverageRating.Value },
        { "AverageRating_NumRatings", AverageRating.NumRatings },
        { "UserId", UserId.Value },
        { "CreatedDateTime", CreatedDateTime },
        { "HostMenuIds", _menuIds.Select(x => new { Keys = new[] {"HostId", "HostMenuId"}, HostId = Id.Value, HostMenuId = x.Value }) },
        { "HostDinnerIds", _dinnerIds.Select(x => new { Keys = new[] {"HostId", "HostDinnerId"}, HostId = Id.Value, HostDinnerId = x.Value }) },
    };
}