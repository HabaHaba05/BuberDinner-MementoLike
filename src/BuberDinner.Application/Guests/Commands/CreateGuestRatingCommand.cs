using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Guests.Commands;
public record CreateGuestRatingCommand(HostId HostId, string GuestId, string DinnerId, int Rating) : IRequest<ErrorOr<Guest>>;