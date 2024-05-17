using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate;
using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Domain.Common.DomainErrors;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Bills.Commands.PayBill;
public class PayBillCommandHandler : IRequestHandler<PayBillCommand, ErrorOr<Bill>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PayBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Bill>> Handle(PayBillCommand request, CancellationToken cancellationToken)
    {
        var billId = BillId.Create(request.BillId);

        if (await _unitOfWork.BillRepository.GetByIdAsync(billId) is not Bill bill)
        {
            return Errors.Bill.NotFound;
        }

        bill.Pay();

        await _unitOfWork.BillRepository.UpdateAsync(bill);
        return bill;
    }
}