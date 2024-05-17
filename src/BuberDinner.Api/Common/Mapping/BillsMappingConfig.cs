using BuberDinner.Contracts.Bills;
using BuberDinner.Contracts.Dinners;
using BuberDinner.Domain.BillAggregate;

using Mapster;

namespace BuberDinner.Api.Common.Mapping;

public class BillsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Bill, BillResponse>()
            .Map(dest => dest.DinnerId, src => src.DinnerId.Value)
            .Map(dest => dest.GuestId, src => src.GuestId.Value)
            .Map(dest => dest.HostId, src => src.HostId.Value)
            .Map(dest => dest.Price, src => new Price(src.Amount.Amount, src.Amount.Currency))
            .Map(dest => dest.Status, src => src.Status.Value)
            .Map(dest => dest.CreatedDateTime, src => src.CreatedDateTime)
            .Map(dest => dest.PaidAt, src => src.PaidAt);
    }
}