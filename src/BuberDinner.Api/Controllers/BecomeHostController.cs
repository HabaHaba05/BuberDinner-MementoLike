using BuberDinner.Api.Extensions;
using BuberDinner.Application.BecomeHost.Commands.CreateRequest;
using BuberDinner.Application.BecomeHost.Commands.UpdateStatus;
using BuberDinner.Application.BecomeHost.Queries.ListPendingBecomeHostRequests;
using BuberDinner.Contracts.BecomeHostRequest;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("becomeHostRequests")]
public class BecomeHostController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public BecomeHostController(ISender mediator, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _mediator = mediator;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    [HttpPost("create")]
    [Authorize(Policy = Constants.Authentication.Policies.User)]
    public async Task<IActionResult> Create()
    {
        var userId = _contextAccessor.GetUserId();
        var response = await _mediator.Send(new CreateBecomeHostRequestCommand(userId!));

        return response.Match(
            request => Ok(_mapper.Map<BecomeHostRequestResponse>(request)),
            errors => Problem(errors));
    }

    [HttpGet("getPendingRequests")]
    [Authorize(Policy = Constants.Authentication.Policies.Admin)]
    public async Task<IActionResult> GetPendingRequests()
    {
        var pendingRequests = await _mediator.Send(new ListPendingBecomeHostRequestsQuery());

        return pendingRequests.Match(
            requests => Ok(requests.Select(request => _mapper.Map<BecomeHostRequestResponse>(request))),
            errors => Problem(errors));
    }

    [HttpPost("approve/{becomeHostRequestId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Admin)]
    public async Task<IActionResult> Approve(string becomeHostRequestId)
    {
        var adminId = _contextAccessor.GetAdminId();
        var response = await _mediator.Send(new UpdateBecomeHostRequestStatusCommand(true, becomeHostRequestId, adminId!));

        return response.Match(
            request => Ok(_mapper.Map<BecomeHostRequestResponse>(request)),
            errors => Problem(errors));
    }

    [HttpPost("reject/{becomeHostRequestId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Admin)]
    public async Task<IActionResult> Reject(string becomeHostRequestId)
    {
        var adminId = _contextAccessor.GetAdminId();
        var response = await _mediator.Send(new UpdateBecomeHostRequestStatusCommand(false, becomeHostRequestId, adminId!));

        return response.Match(
            request => Ok(_mapper.Map<BecomeHostRequestResponse>(request)),
            errors => Problem(errors));
    }
}