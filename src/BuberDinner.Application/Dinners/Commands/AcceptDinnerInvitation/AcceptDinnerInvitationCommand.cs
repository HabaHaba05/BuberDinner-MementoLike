using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.AcceptDinnerInvitation;
public record AcceptDinnerInvitationCommand(GuestId GuestId, string ReservationId) : IRequest<ErrorOr<Reservation>>;