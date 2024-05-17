using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Queries.ListPendingBecomeHostRequests;
public class ListPendingBecomeHostRequestsQueryHandler : IRequestHandler<ListPendingBecomeHostRequestsQuery, ErrorOr<List<BecomeHostRequest>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListPendingBecomeHostRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<List<BecomeHostRequest>>> Handle(ListPendingBecomeHostRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.BecomeHostRequestRepository.ListPendingRequestsAsync();
    }
}