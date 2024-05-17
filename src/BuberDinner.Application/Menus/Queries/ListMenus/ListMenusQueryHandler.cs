using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Menus.Queries.ListMenus;

public class ListMenusQueryHandler : IRequestHandler<ListMenusQuery, ErrorOr<List<Menu>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListMenusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<List<Menu>>> Handle(ListMenusQuery query, CancellationToken cancellationToken)
    {
        var hostId = HostId.Create(query.HostId);
        if (hostId.IsError)
        {
            return hostId.Errors;
        }

        return await _unitOfWork.MenuRepository.ListAsync(hostId.Value);
    }
}