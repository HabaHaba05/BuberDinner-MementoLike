using Ardalis.GuardClauses;

using BuberDinner.SharedKernel;

namespace BuberDinner.Domain.Common.ValueObjects;

public sealed class Rating : ValueObject
{
    public Rating(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }

    public static Rating Create(int value)
    {
        Guard.Against.Expression(x => x < 1 || x > 5, value, "Rating allowed values are from 1 to 5");
        return new Rating(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}