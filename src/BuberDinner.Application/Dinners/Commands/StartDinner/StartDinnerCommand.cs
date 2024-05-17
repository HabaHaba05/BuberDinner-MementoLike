using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.StartDinner;
public record StartDinnerCommand(HostId HostId, string DinnerId) : IRequest<ErrorOr<Dinner>>;