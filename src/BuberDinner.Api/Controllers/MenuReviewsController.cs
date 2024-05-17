using BuberDinner.Api.Extensions;
using BuberDinner.Application.MenuReviews.Commands.CreateMenuReview;
using BuberDinner.Contracts.MenuReviews;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("menuReviews")]
[Authorize]
public class MenuReviewsController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public MenuReviewsController(IMapper mapper, ISender mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Policy = Constants.Authentication.Policies.Guest)]
    public async Task<IActionResult> PostMenuReview(PostMenuReviewRequest request)
    {
        var guestId = _httpContextAccessor.GetGuestId()!;
        var ratingResult = await _mediator.Send(new CreateMenuReviewCommand(
            guestId, request.DinnerId, request.Comment, request.Rating));

        return ratingResult.Match(
            guest => Ok(),
            errors => Problem(errors));
    }
}