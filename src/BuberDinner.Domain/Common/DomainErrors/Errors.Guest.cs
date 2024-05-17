using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;
public static partial class Errors
{
    public static class Guest
    {
        public static Error InvalidGuestId => Error.Validation(
            code: "Guest.InvalidId",
            description: "Guest ID is invalid");

        public static Error NotFound => Error.NotFound(
            code: "Guest.NotFound",
            description: "Guest with given ID does not exist");
    }
}