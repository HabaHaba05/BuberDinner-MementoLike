using BuberDinner.Api.Extensions;
using BuberDinner.Application.Menus.Commands.CreateMenu;
using BuberDinner.Application.Menus.Queries.ListMenus;
using BuberDinner.Contracts.Menus;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("menus")]
[Authorize]
public class MenusController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public MenusController(IMapper mapper, ISender mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> CreateMenu(CreateMenuRequest request)
    {
        var hostId = _httpContextAccessor.GetHostId();
        var command = _mapper.Map<CreateMenuCommand>((request, hostId));

        var createMenuResult = await _mediator.Send(command);

        return createMenuResult.Match(
            menu => Ok(_mapper.Map<MenuResponse>(menu)),
            errors => Problem(errors));
    }

    [HttpGet("{hostId}")]
    public async Task<IActionResult> ListMenus(string hostId)
    {
        var query = _mapper.Map<ListMenusQuery>(hostId);

        var listMenusResult = await _mediator.Send(query);

        return listMenusResult.Match(
            menus => Ok(menus.Select(menu => _mapper.Map<MenuResponse>(menu))),
            errors => Problem(errors));
    }
}