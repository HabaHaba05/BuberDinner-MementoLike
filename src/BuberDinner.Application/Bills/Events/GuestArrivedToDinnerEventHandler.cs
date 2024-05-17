using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.Events;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;

using MediatR;

namespace BuberDinner.Application.Bills.Events;
public class GuestArrivedToDinnerEventHandler : INotificationHandler<GuestArrivedToDinner>
{
    private readonly IUnitOfWork _unitOfWork;

    public GuestArrivedToDinnerEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(GuestArrivedToDinner guestArrivedToDinnerEvent, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.DinnerRepository.GetByIdAsync(guestArrivedToDinnerEvent.DinnerId) is not Dinner dinner)
        {
            throw new InvalidOperationException($"Dinner not found. Dinner Id: {guestArrivedToDinnerEvent.DinnerId}).");
        }

        var guestId = guestArrivedToDinnerEvent.GuestId;

        var guestReservation = dinner.ArrivedReservations.Single(x => x.GuestId == guestId);

        var price = Price.Create(
            dinner.Price.Amount * guestReservation.GuestCount,
            dinner.Price.Currency);

        var bill = Bill.Create(
            dinner.Id,
            guestId,
            dinner.HostId,
            price);

        await _unitOfWork.BillRepository.AddAsync(bill);
    }
}