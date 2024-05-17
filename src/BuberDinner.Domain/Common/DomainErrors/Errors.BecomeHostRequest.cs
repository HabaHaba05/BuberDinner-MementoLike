using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class BecomeHostRequest
    {
        public static Error PendingRequestAlreadyExist => Error.Validation(
            code: "BecomeHostRequest.PendingRequestAlreadyExist ",
            description: "User already has Pending BecomeHostRequest Already");

        public static Error UserIsAlreadyHost => Error.Validation(
            code: "BecomeHostRequest.UserIsAlreadyHost",
            description: "User is already a host");

        public static Error StatusUpdateNotAllowed => Error.Validation(
            code: "BecomeHostRequest.StatusUpdateNotAllowed",
            description: "BecomeHostRequest Status Update Not Allowed");

        public static Error InvalidBecomeHostRequestId => Error.Validation(
            code: "BecomeHostRequest.InvalidId",
            description: "BecomeHostRequest ID is invalid");

        public static Error NotFound => Error.NotFound(
            code: "BecomeHostRequest.NotFound",
            description: "BecomeHostRequest with given ID does not exist");
    }
}