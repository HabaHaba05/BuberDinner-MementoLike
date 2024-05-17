using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class Host
    {
        public static Error InvalidHostId => Error.Validation(
            code: "Host.InvalidId",
            description: "Host ID is invalid");

        public static Error NotFound => Error.NotFound(
            code: "Host.NotFound",
            description: "Host with given ID does not exist");
    }
}