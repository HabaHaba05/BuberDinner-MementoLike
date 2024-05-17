using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.StartDinner;
public class StartDinnerCommandHandler : IRequestHandler<StartDinnerCommand, ErrorOr<Dinner>>
{
    private readonly IUnitOfWork _unitOfWork;
    public StartDinnerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Dinner>> Handle(StartDinnerCommand request, CancellationToken cancellationToken)
    {
        var dinnerId = DinnerId.Create(request.DinnerId);
        if (dinnerId.IsError)
        {
            return dinnerId.Errors;
        }

        if (await _unitOfWork.DinnerRepository.GetByIdAsync(dinnerId.Value) is not Dinner dinner)
        {
            return Errors.Dinner.NotFound;
        }

        var startDinnerResult = dinner.Start(request.HostId);
        if (startDinnerResult.IsError)
        {
            return startDinnerResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return dinner;
    }
}