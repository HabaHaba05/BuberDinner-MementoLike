using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.BecomeHostRequestAggregate.Enums;
using BuberDinner.Domain.Common.DomainErrors;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.BecomeHost.Commands.CreateRequest;
public class CreateBecomeHostRequestCommandHandler : IRequestHandler<CreateBecomeHostRequestCommand, ErrorOr<BecomeHostRequest>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBecomeHostRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<BecomeHostRequest>> Handle(CreateBecomeHostRequestCommand request, CancellationToken cancellationToken)
    {
        var userRequests = await _unitOfWork.BecomeHostRequestRepository.GetByUserIdAsync(request.UserId);

        if (userRequests.Any(x => x.Status == RequestResponseType.Pending))
        {
            return Errors.BecomeHostRequest.PendingRequestAlreadyExist;
        }
        else if (userRequests.Any(x => x.Status == RequestResponseType.Approved))
        {
            return Errors.BecomeHostRequest.UserIsAlreadyHost;
        }

        var becomeHostRequest = BecomeHostRequest.Create(request.UserId);
        await _unitOfWork.BecomeHostRequestRepository.AddAsync(becomeHostRequest);
        return becomeHostRequest;
    }
}