using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.FinishDinner;

public record FinishDinnerCommand(HostId HostId, string DinnerId) : IRequest<ErrorOr<Dinner>>;