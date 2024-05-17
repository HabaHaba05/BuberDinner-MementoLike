using System.Data;

namespace BuberDinner.Infrastructure.Persistence;
public abstract class RepositoryBase
{
    protected IDbTransaction Transaction { get; init; }
    protected IDbConnection Connection
    {
        get { return Transaction.Connection!; }
    }

    public RepositoryBase(IDbTransaction transaction)
    {
        Transaction = transaction;
    }
}