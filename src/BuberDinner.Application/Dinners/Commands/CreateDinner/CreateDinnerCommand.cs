using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Dinners.Commands.CreateDinner;

public record CreateDinnerCommand(
    HostId HostId,
    string Name,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    bool IsPublic,
    int MaxGuests,
    DinnerPriceCommand Price,
    string MenuId,
    DinnerLocationCommand Location) : IRequest<ErrorOr<Dinner>>;

public record DinnerPriceCommand(
    decimal Amount,
    string Currency);

public record DinnerLocationCommand(
    string Name,
    string Address,
    double Latitude,
    double Longitude);