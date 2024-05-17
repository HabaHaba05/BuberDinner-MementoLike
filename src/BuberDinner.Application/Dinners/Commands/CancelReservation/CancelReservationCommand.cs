using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CancelReservation;
public record CancelReservationCommand(string ReservationId, GuestId GuestId) : IRequest<ErrorOr<Reservation>>;