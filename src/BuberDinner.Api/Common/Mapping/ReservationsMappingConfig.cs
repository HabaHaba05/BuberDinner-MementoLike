using BuberDinner.Application.Dinners.Commands.CreateDinnerReservation;
using BuberDinner.Application.Dinners.Commands.InviteGuestToDinner;
using BuberDinner.Contracts.Dinners;
using BuberDinner.Domain.DinnerAggregate.Entities;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;

using Mapster;

namespace BuberDinner.Api.Common.Mapping;

public class ReservationsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(DinnerReservationRequest Req, GuestId GuestId), CreateDinnerReservationCommand>()
            .Map(dest => dest.GuestId, src => src.GuestId)
            .Map(dest => dest.DinnerId, src => src.Req.DinnerId)
            .Map(dest => dest.GuestsCount, src => src.Req.GuestsCount);

        config.NewConfig<(InviteGuestToDinnerRequest Req, HostId HostId), InviteGuestToDinnerCommand>()
            .Map(dest => dest.HostId, src => src.HostId)
            .Map(dest => dest.DinnerId, src => src.Req.DinnerId)
            .Map(dest => dest.GuestId, src => src.Req.GuestId);

        config.NewConfig<Reservation, ReservationResponse>()
            .Map(dest => dest.ReservationId, src => src.Id.Value)
            .Map(dest => dest.GuestCount, src => src.GuestCount)
            .Map(dest => dest.GuestId, src => src.GuestId.Value)
            .Map(dest => dest.Status, src => src.Status.Value)
            .Map(dest => dest.ArrivalDateTime, src => src.ArrivalDateTime)
            .Map(dest => dest.CreatedDateTime, src => src.CreatedDateTime)
            .Map(dest => dest.UpdatedDateTime, src => src.UpdatedDateTime);
    }
}