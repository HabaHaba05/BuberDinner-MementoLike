using BuberDinner.Application.Menus.Commands.CreateMenu;
using BuberDinner.Application.Menus.Queries.ListMenus;
using BuberDinner.Contracts.Menus;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.MenuAggregate;

using Mapster;

using MenuItem = BuberDinner.Domain.MenuAggregate.Entities.MenuItem;
using MenuSection = BuberDinner.Domain.MenuAggregate.Entities.MenuSection;

namespace BuberDinner.Api.Common.Mapping;

public class MenuMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CreateMenuRequest Request, HostId HostId), CreateMenuCommand>()
            .Map(dest => dest.HostId, src => src.HostId)
            .Map(dest => dest.Name, src => src.Request.Name)
            .Map(dest => dest, src => src.Request);

        config.NewConfig<string, ListMenusQuery>()
            .MapWith(src => new ListMenusQuery(src));

        config.NewConfig<Menu, MenuResponse>()
            .Map(dest => dest.Id, src => src.Id.Value.ToString())
            .Map(dest => dest.AverageRating, src => src.AverageRating.Value)
            .Map(dest => dest.HostId, src => src.HostId.Value)
            .Map(dest => dest.DinnerIds, src => src.DinnerIds.Select(dinnerId => dinnerId.Value))
            .Map(dest => dest.MenuReviewIds, src => src.MenuReviewIds.Select(menuId => menuId.Value));

        config.NewConfig<MenuSection, MenuSectionResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        config.NewConfig<MenuItem, MenuItemResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);
    }
}