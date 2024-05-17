using BuberDinner.Api.Extensions;
using BuberDinner.Application.Dinners.Commands.CreateDinner;
using BuberDinner.Application.Dinners.Commands.FinishDinner;
using BuberDinner.Application.Dinners.Commands.GuestArrived;
using BuberDinner.Application.Dinners.Commands.StartDinner;
using BuberDinner.Application.Dinners.Queries.ListDinners;
using BuberDinner.Contracts.Dinners;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("dinners")]
[Authorize]
public class DinnersController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DinnersController(IMapper mapper, ISender mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> CreateDinner(CreateDinnerRequest request)
    {
        var hostId = _httpContextAccessor.GetHostId();
        var command = _mapper.Map<CreateDinnerCommand>((request, hostId));

        var createDinnerResult = await _mediator.Send(command);

        return createDinnerResult.Match(
            dinner => Ok(_mapper.Map<DinnerResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("start/{dinnerId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> StartDinner(string dinnerId)
    {
        var hostId = _httpContextAccessor.GetHostId()!;
        var dinner = await _mediator.Send(new StartDinnerCommand(hostId, dinnerId));

        return dinner.Match(
            dinners => Ok(_mapper.Map<DinnerResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("finish/{dinnerId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> FinishDinner(string dinnerId)
    {
        var hostId = _httpContextAccessor.GetHostId()!;
        var dinner = await _mediator.Send(new FinishDinnerCommand(hostId, dinnerId));

        return dinner.Match(
            dinners => Ok(_mapper.Map<DinnerResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("guestArrived")]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> GuestArrived(GuestArrivedRequet request)
    {
        var hostId = _httpContextAccessor.GetHostId()!;
        var dinner = await _mediator.Send(new GuestArrivedCommand(hostId, request.GuestId, request.DinnerId));

        return dinner.Match(
            dinners => Ok(_mapper.Map<DinnerResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpGet("{hostId}")]
    public async Task<IActionResult> ListDinners(string hostId)
    {
        var query = _mapper.Map<ListDinnersQuery>(hostId);

        var listDinnersQuery = await _mediator.Send(query);

        return listDinnersQuery.Match(
            dinners => Ok(dinners.Select(dinner => _mapper.Map<DinnerResponse>(dinner))),
            errors => Problem(errors));
    }
}