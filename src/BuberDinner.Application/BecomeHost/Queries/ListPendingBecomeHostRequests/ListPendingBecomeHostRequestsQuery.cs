using BuberDinner.Domain.BecomeHostRequestAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Queries.ListPendingBecomeHostRequests;

public record ListPendingBecomeHostRequestsQuery() : IRequest<ErrorOr<List<BecomeHostRequest>>>;