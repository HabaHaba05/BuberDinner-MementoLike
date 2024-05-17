using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CreateDinnerReservation;
public record CreateDinnerReservationCommand(GuestId GuestId, string DinnerId, int GuestsCount) : IRequest<ErrorOr<Reservation>>;