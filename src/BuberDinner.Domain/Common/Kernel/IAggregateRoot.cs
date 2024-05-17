namespace BuberDinner.SharedKernel;
public interface IAggregateRoot : IHasDomainEvents
{
    // https://stackoverflow.com/questions/75686568/the-interface-cannot-be-used-as-type-argument-static-member-does-not-have-a-mos
    public static virtual string TableName => throw new NotImplementedException();
    Dictionary<string, object?> GetState();
}