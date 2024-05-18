using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;
using BuberDinner.Domain.Common.DomainErrors;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Commands.UpdateStatus;
public class UpdateBecomeHostRequestStatusCommandHandler : IRequestHandler<UpdateBecomeHostRequestStatusCommand, ErrorOr<BecomeHostRequest>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBecomeHostRequestStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<BecomeHostRequest>> Handle(UpdateBecomeHostRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var becomeHostRequestId = BecomeHostRequestId.Create(request.BecomeHostRequestId);
        if (becomeHostRequestId.IsError)
        {
            return becomeHostRequestId.Errors;
        }

        var becomeHostRequest = await _unitOfWork.BecomeHostRequestRepository.GetByIdAsync(becomeHostRequestId.Value);

        if (becomeHostRequest is null)
        {
            return Errors.BecomeHostRequest.NotFound;
        }

        var updateReponse = request.Approve
            ? becomeHostRequest.Approve(request.AdminId)
            : becomeHostRequest.Reject(request.AdminId);

        if (updateReponse.HasValue)
        {
            return updateReponse.Value;
        }

        await _unitOfWork.BecomeHostRequestRepository.UpdateAsync(becomeHostRequest);
        return becomeHostRequest;
    }
}