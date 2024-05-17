using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;
public static partial class Errors
{
    public static class Dinner
    {
        public static Error DinnerBelongsToDifferentHost => Error.Validation(
            code: "Dinner.DinnerBelongsToDifferentHost",
            description: "Dinner belongs to different Host");

        public static Error ReservationsCannotBeUpdated => Error.Validation(
            code: "Dinner.ReservationsCannotBeUpdated",
            description: "Dinner reservations can not be updated.");

        public static Error DinnerDoesNotContainProvidedReservation => Error.Validation(
            code: "Dinner.DinnerDoesNotContainProvidedReservation",
            description: "Dinner does not contain provided reservation.");

        public static Error DinnerNotFoundByReservationId => Error.Validation(
            code: "Dinner.DinnerNotFoundByReservationId",
            description: "Could not find a dinner for provided reservation Id.");

        public static Error GuestAlreadyHasReservationToThisDinner => Error.Validation(
            code: "Dinner.GuestAlreadyHasReservationToThisDinner",
            description: "Guest already has a reservation to this dinner.");

        public static Error MaxCapacityReached => Error.Validation(
            code: "Dinner.MaxCapacityReached ",
            description: "There are less empty seats, than requested.");

        public static Error InvalidDinnerId => Error.Validation(
            code: "Dinner.InvalidId",
            description: "Dinner ID is invalid.");

        public static Error NotFound => Error.NotFound(
            code: "Dinner.NotFound",
            description: "Dinner with given ID does not exist.");
    }
}