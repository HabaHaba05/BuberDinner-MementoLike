using Ardalis.GuardClauses;

using BuberDinner.Domain.AdminAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.AdminAggregate;

public class Admin : AggregateRoot<AdminId, Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    private Admin(string name, string email, string password, AdminId? adminId = null)
        : base(adminId ?? AdminId.CreateUnique())
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public static Admin Create(string name, string email, string password)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.NullOrEmpty(email);
        Guard.Against.NullOrEmpty(password);

        return new Admin(
            name,
            email,
            password);
    }

    public static string TableName => "Admins";

    public static Admin FromState(Dictionary<string, object?> state) => new(
        (string)state["Name"]!,
        (string)state["Email"]!,
        (string)state["Password"]!,
        AdminId.Create((Guid)state["Id"]!));

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value.ToString() },
        { "Name", Name },
        { "Email", Email },
        { "Password", Password },
    };
}