using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class Bill
    {
        public static Error NotFound => Error.NotFound(
            code: "Bill.NotFound",
            description: "Bill with given ID does not exist");
    }
}