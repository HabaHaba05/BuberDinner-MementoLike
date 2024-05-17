using BuberDinner.Domain.AdminAggregate.ValueObjects;
using BuberDinner.Domain.BecomeHostRequestAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Commands.UpdateStatus;
public record UpdateBecomeHostRequestStatusCommand(bool Approve, string BecomeHostRequestId, AdminId AdminId) : IRequest<ErrorOr<BecomeHostRequest>>;