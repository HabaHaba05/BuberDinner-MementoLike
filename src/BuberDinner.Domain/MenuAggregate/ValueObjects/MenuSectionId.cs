using BuberDinner.Domain.Common.Models;

namespace BuberDinner.Domain.MenuAggregate.ValueObjects;

public sealed class MenuSectionId : EntityId<Guid>
{
    public MenuSectionId(Guid value) : base(value)
    {
    }

    public static MenuSectionId CreateUnique()
    {
        // TODO: enforce invariants
        return new MenuSectionId(Guid.NewGuid());
    }

    public static MenuSectionId Create(Guid value)
    {
        // TODO: enforce invariants
        return new MenuSectionId(value);
    }
}