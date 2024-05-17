using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.InviteGuestToDinner;

public record InviteGuestToDinnerCommand(string GuestId, HostId HostId, string DinnerId) : IRequest<ErrorOr<Reservation>>;