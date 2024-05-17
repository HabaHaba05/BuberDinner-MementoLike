using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.UserAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Commands.CreateRequest;
public record CreateBecomeHostRequestCommand(UserId UserId) : IRequest<ErrorOr<BecomeHostRequest>>;