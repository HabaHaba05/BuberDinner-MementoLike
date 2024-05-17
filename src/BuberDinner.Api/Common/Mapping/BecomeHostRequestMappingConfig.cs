using BuberDinner.Contracts.BecomeHostRequest;
using BuberDinner.Domain.BecomeHostRequestAggregate;

using Mapster;

namespace BuberDinner.Api.Common.Mapping;

public class BecomeHostRequestMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BecomeHostRequest, BecomeHostRequestResponse>()
            .Map(dest => dest.Id, src => src.Id.Value.ToString())
            .Map(dest => dest.Status, src => src.Status.Name)
            .Map(dest => dest.UserId, src => src.UserId.Value.ToString())
            .Map(dest => dest.CreatedAt, src => src.CreatedDateTime)
            .Map(dest => dest.ReviewedByAdminId, src => src.ReviewedByAdminId)
            .Map(dest => dest.ReviewedAt, src => src.ReviewedAt);
    }
}