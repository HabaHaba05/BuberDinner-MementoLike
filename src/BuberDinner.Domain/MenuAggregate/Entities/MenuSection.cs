using Ardalis.GuardClauses;

using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuAggregate.Entities;

public sealed class MenuSection : Entity<MenuSectionId>
{
    private readonly List<MenuItem> _items;
    private readonly MenuId _menuId;
    public string Name { get; private set; }
    public string Description { get; private set; }

    public IReadOnlyList<MenuItem> Items => _items.AsReadOnly();

    private MenuSection(
        string name,
        string description,
        List<MenuItem> items,
        MenuSectionId id,
        MenuId menuId)
        : base(id)
    {
        Name = name;
        Description = description;
        _items = items;
        _menuId = menuId;
    }

    public static MenuSection Create(
        MenuSectionId menuSectionId,
        string name,
        string description,
        MenuId menuId,
        List<MenuItem>? items = null)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.NullOrEmpty(description);
        return new MenuSection(name, description, items ?? new(), menuSectionId, menuId);
    }

    public static MenuSection FromState(Dictionary<string, object?> state) => new(
        (string)state["Name"]!,
        (string)state["Description"]!,
        ((IEnumerable<IDictionary<string, object?>>)state["MenuItems"]!).Select(x => MenuItem.FromState(x.ToDictionary())).ToList(),
        MenuSectionId.Create((Guid)state["MenuSectionId"]!),
        MenuId.Create((Guid)state["MenuId"]!));

    public Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "MenuSectionId", "MenuId" } },
        { "MenuSectionId", Id.Value },
        { "MenuId", _menuId.Value },
        { "Name", Name },
        { "Description", Description },
        { "MenuItems", _items.Select(x => x.GetState()) },
    };
}