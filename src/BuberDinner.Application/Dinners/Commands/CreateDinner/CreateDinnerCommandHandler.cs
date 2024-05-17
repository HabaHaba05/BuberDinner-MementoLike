using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CreateDinner;

public class CreateDinnerCommandHandler : IRequestHandler<CreateDinnerCommand, ErrorOr<Dinner>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDinnerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Dinner>> Handle(CreateDinnerCommand command, CancellationToken cancellationToken)
    {
        var createMenuIdResult = MenuId.Create(command.MenuId);

        if (createMenuIdResult.IsError)
        {
            return createMenuIdResult.Errors;
        }

        if (!await _unitOfWork.MenuRepository.ExistsAsync(createMenuIdResult.Value))
        {
            return Errors.Menu.NotFound;
        }

        var dinner = Dinner.Create(
            command.Name,
            command.Description,
            command.StartDateTime,
            command.EndDateTime,
            command.IsPublic,
            command.MaxGuests,
            Price.Create(
                command.Price.Amount,
                command.Price.Currency),
            createMenuIdResult.Value,
            command.HostId,
            Location.Create(
                command.Location.Name,
                command.Location.Address,
                command.Location.Latitude,
                command.Location.Longitude));

        await _unitOfWork.DinnerRepository.AddAsync(dinner);

        return dinner;
    }
}