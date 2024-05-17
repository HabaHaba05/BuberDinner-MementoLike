using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate.Events;
using BuberDinner.Domain.DinnerAggregate;

using MediatR;

namespace BuberDinner.Application.Dinners.Events;

public class BillCreatedEventHandler : INotificationHandler<BillCreated>
{
    private readonly IUnitOfWork _unitOfWork;

    public BillCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BillCreated billCreatedEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.DinnerRepository.GetByIdAsync(billCreatedEvent.Bill.DinnerId) is not Dinner dinner)
        {
            throw new InvalidOperationException($"Dinner Not found in database. Dinner Id: {billCreatedEvent.Bill.DinnerId}");
        }

        var reservation = dinner.ArrivedReservations.Single(x => x.GuestId == billCreatedEvent.Bill.GuestId);

        reservation.AddBill(billCreatedEvent.Bill.Id);
        await _unitOfWork.DinnerRepository.UpdateAsync(dinner);
    }
}