namespace BuberDinner.SharedKernel;

public abstract class AggregateRoot<TId, TIdType> : Entity<TId>, IAggregateRoot
    where TId : AggregateRootId<TIdType>
{
    // public new AggregateRootId<TIdType> Id { get; protected set; }
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    public abstract Dictionary<string, object?> GetState();

#pragma warning disable CS8618
    protected AggregateRoot()
    {
    }
#pragma warning restore CS8618
}