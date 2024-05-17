using BuberDinner.Api.Extensions;
using BuberDinner.Application.Guests.Commands;
using BuberDinner.Contracts.Guests;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("guestRatings")]
[Authorize]
public class GuestRatingsController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GuestRatingsController(IMapper mapper, ISender mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> CreateGuestRating(CreateGuestRatingRequest request)
    {
        var hostId = _httpContextAccessor.GetHostId()!;
        var ratingResult = await _mediator.Send(new CreateGuestRatingCommand(
            hostId, request.GuestId, request.DinnerId, request.Rating));

        return ratingResult.Match(
            guest => Ok(),
            errors => Problem(errors));
    }
}