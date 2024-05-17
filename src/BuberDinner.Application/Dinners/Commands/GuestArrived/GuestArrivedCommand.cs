using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.GuestArrived;
public record GuestArrivedCommand(HostId HostId, string GuestId, string DinnerId) : IRequest<ErrorOr<Dinner>>;