using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Domain.MenuAggregate.Entities;
using BuberDinner.Domain.MenuAggregate.ValueObjects;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Menus.Commands.CreateMenu;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, ErrorOr<Menu>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMenuCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Menu>> Handle(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        var menuId = MenuId.CreateUnique();
        var menuSectionsIds = command.Sections.ConvertAll(_ => MenuSectionId.CreateUnique());
        var menu = Menu.Create(
            menuId,
            hostId: command.HostId,
            name: command.Name,
            description: command.Description,
            sections: command.Sections.Select((section, i) => MenuSection.Create(
                menuSectionsIds[i],
                section.Name,
                section.Description,
                menuId,
                section.Items.ConvertAll(item => MenuItem.Create(
                    item.Name,
                    item.Description,
                    menuId,
                    menuSectionsIds[i])))).ToList());

        await _unitOfWork.MenuRepository.AddAsync(menu);

        return menu;
    }
}