using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.RejectDinnerInvitation;

public record RejectDinnerInvitationCommand(GuestId GuestId, string ReservationId) : IRequest<ErrorOr<Reservation>>;