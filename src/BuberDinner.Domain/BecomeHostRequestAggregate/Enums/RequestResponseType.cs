using Ardalis.SmartEnum;

namespace BuberDinner.Domain.BecomeHostRequestAggregate.Enums;

public class RequestResponseType : SmartEnum<RequestResponseType>
{
    public RequestResponseType(string name, int value) : base(name, value)
    {
    }

    public static readonly RequestResponseType Pending = new(nameof(Pending), 1);
    public static readonly RequestResponseType Rejected = new(nameof(Rejected), 2);
    public static readonly RequestResponseType Approved = new(nameof(Approved), 3);
}