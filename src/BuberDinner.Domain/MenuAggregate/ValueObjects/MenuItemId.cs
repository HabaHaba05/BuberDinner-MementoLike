using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.MenuAggregate.ValueObjects;

public sealed class MenuItemId : EntityId<Guid>
{
    private MenuItemId(Guid value) : base(value)
    {
    }

    public static MenuItemId Create(Guid value)
    {
        return new MenuItemId(value);
    }

    public static MenuItemId CreateUnique()
    {
        return new MenuItemId(Guid.NewGuid());
    }
}