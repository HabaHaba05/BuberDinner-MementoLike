using Ardalis.GuardClauses;

using BuberDinner.Domain.MenuAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.MenuAggregate.Entities;

public sealed class MenuItem : Entity<MenuItemId>
{
    private readonly MenuId _menuId;
    private readonly MenuSectionId _menuSectionId;
    public string Name { get; private set; }
    public string Description { get; private set; }

    private MenuItem(
        string name,
        string description,
        MenuId menuId,
        MenuSectionId menuSectionId,
        MenuItemId id)
        : base(id)
    {
        Name = name;
        Description = description;
        _menuId = menuId;
        _menuSectionId = menuSectionId;
    }

    public static MenuItem Create(
        string name,
        string description,
        MenuId menuId,
        MenuSectionId menuSectionId)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.NullOrEmpty(description);
        return new MenuItem(name, description, menuId, menuSectionId, MenuItemId.CreateUnique());
    }

    public static MenuItem FromState(Dictionary<string, object?> state) => new(
            (string)state["Name"]!,
            (string)state["Description"]!,
            MenuId.Create((Guid)state["MenuId"]!),
            MenuSectionId.Create((Guid)state["MenuSectionId"]!),
            MenuItemId.Create((Guid)state["MenuItemId"]!));

    public Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "MenuItemId", "MenuId", "MenuSectionId" } },
        { "MenuItemId", Id.Value },
        { "MenuId", _menuId.Value },
        { "MenuSectionId", _menuSectionId.Value },
        { "Name", Name },
        { "Description", Description },
    };
}