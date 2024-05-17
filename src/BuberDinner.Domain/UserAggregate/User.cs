using Ardalis.GuardClauses;

using BuberDinner.Domain.UserAggregate.Events;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.UserAggregate;

public sealed class User : AggregateRoot<UserId, Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; } // TODO: Hash this

    public DateTime CreatedDateTime { get; private set; }

    private User(string firstName, string lastName, string email, string password, UserId userId, DateTime createdDateTime)
        : base(userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        CreatedDateTime = createdDateTime;
    }

    public static User Create(string firstName, string lastName, string email, string password)
    {
        Guard.Against.NullOrEmpty(firstName);
        Guard.Against.NullOrEmpty(lastName);
        Guard.Against.NullOrEmpty(email);
        Guard.Against.NullOrEmpty(password);

        var user = new User(
            firstName,
            lastName,
            email,
            password,
            UserId.CreateUnique(),
            DateTime.UtcNow);

        user.AddDomainEvent(new UserCreated(user));
        return user;
    }

    public static string TableName => "Users";

    public static User FromState(Dictionary<string, object?> state) => new(
        (string)state["FirstName"]!,
        (string)state["LastName"]!,
        (string)state["Email"]!,
        (string)state["Password"]!,
        UserId.Create((Guid)state["Id"]!),
        (DateTime)state["CreatedDateTime"]!);

    public override Dictionary<string, object?> GetState() => new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Id.Value.ToString() },
        { "FirstName", FirstName },
        { "LastName", LastName },
        { "Email", Email },
        { "Password", Password },
        { "CreatedDateTime", CreatedDateTime },
    };
}