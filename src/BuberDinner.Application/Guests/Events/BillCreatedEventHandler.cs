using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate.Events;
using BuberDinner.Domain.GuestAggregate;

using MediatR;

namespace BuberDinner.Application.Guests.Events;

public class BillCreatedEventHandler : INotificationHandler<BillCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public BillCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BillCreated billCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.GuestRepository.GetByIdAsync(billCreatedEvent.Bill.GuestId) is not Guest guest)
        {
            throw new InvalidOperationException($"Guest Not found in database. Guest Id: {billCreatedEvent.Bill.GuestId}");
        }

        guest.AddBill(billCreatedEvent.Bill.Id);
        await _unitOfWork.GuestRepository.UpdateAsync(guest);
    }
}