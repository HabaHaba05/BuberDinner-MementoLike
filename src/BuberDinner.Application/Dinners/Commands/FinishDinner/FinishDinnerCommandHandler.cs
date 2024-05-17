using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Dinners.Commands.StartDinner;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.FinishDinner;

public class FinishDinnerCommandHandler : IRequestHandler<FinishDinnerCommand, ErrorOr<Dinner>>
{
    private readonly IUnitOfWork _unitOfWork;
    public FinishDinnerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Dinner>> Handle(FinishDinnerCommand request, CancellationToken cancellationToken)
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

        var finishDinnerResult = dinner.Finish(request.HostId);
        if (finishDinnerResult.IsError)
        {
            return finishDinnerResult.Errors;
        }

        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
        return dinner;
    }
}