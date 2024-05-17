using ErrorOr;

namespace BuberDinner.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class Reservation
    {
        public static Error ReservationDoesNotBelongToGuest => Error.Validation(
            code: "Reservation.ReservationDoesNotBelongToGuest",
            description: "Reservation Does Not Belong To Guest");

        public static Error GuestCountCannotBeLessThanOne => Error.Validation(
            code: "Reservation.GuestCount",
            description: "Guest Count must be greater than 0");

        public static Error InvalidReservationId => Error.Validation(
            code: "Reservation.InvalidId",
            description: "Reservation ID is invalid");

        public static Error NotFound => Error.NotFound(
            code: "Reservation.NotFound",
            description: "Reservation with given ID does not exist");
    }
}