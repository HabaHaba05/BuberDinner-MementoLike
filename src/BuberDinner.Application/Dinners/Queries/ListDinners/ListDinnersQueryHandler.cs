using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Queries.ListDinners;

public class ListDinnersQueryHandler : IRequestHandler<ListDinnersQuery, ErrorOr<List<Dinner>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListDinnersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<List<Dinner>>> Handle(ListDinnersQuery request, CancellationToken cancellationToken)
    {
        var hostId = HostId.Create(request.HostId);
        if (hostId.IsError)
        {
            return hostId.Errors;
        }

        return await _unitOfWork.DinnerRepository.ListAsync(hostId.Value);
    }
}