using BuberDinner.Api.Extensions;
using BuberDinner.Application.Dinners.Commands.AcceptDinnerInvitation;
using BuberDinner.Application.Dinners.Commands.CancelReservation;
using BuberDinner.Application.Dinners.Commands.CreateDinnerReservation;
using BuberDinner.Application.Dinners.Commands.InviteGuestToDinner;
using BuberDinner.Application.Dinners.Commands.RejectDinnerInvitation;
using BuberDinner.Contracts.Dinners;
using BuberDinner.Infrastructure;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("reservations")]
public class ReservationsController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReservationsController(IMapper mapper, ISender mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Policy = Constants.Authentication.Policies.Guest)]
    public async Task<IActionResult> ReserveDinner(DinnerReservationRequest request)
    {
        var guestId = _httpContextAccessor.GetGuestId()!;

        var createDinnerReservationResult = await _mediator.Send(new CreateDinnerReservationCommand(
            guestId, request.DinnerId, request.GuestsCount));

        return createDinnerReservationResult.Match(
            dinner => Ok(_mapper.Map<ReservationResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("cancel/{reservationId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Guest)]
    public async Task<IActionResult> CancelReservation(string reservationId)
    {
        var guestId = _httpContextAccessor.GetGuestId()!;
        var cancelReservationResult = await _mediator.Send(new CancelReservationCommand(reservationId, guestId));

        return cancelReservationResult.Match(
            dinner => Ok(_mapper.Map<ReservationResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("accept/{reservationId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Guest)]
    public async Task<IActionResult> AcceptReservation(string reservationId)
    {
        var guestId = _httpContextAccessor.GetGuestId()!;
        var acceptInvitationResult = await _mediator.Send(new AcceptDinnerInvitationCommand(guestId, reservationId));

        return acceptInvitationResult.Match(
            dinner => Ok(_mapper.Map<ReservationResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("reject/{reservationId}")]
    [Authorize(Policy = Constants.Authentication.Policies.Guest)]
    public async Task<IActionResult> RejectReservation(string reservationId)
    {
        var guestId = _httpContextAccessor.GetGuestId()!;
        var rejectInvitationResult = await _mediator.Send(new RejectDinnerInvitationCommand(guestId, reservationId));

        return rejectInvitationResult.Match(
            dinner => Ok(_mapper.Map<ReservationResponse>(dinner)),
            errors => Problem(errors));
    }

    [HttpPost("invite")]
    [Authorize(Policy = Constants.Authentication.Policies.Host)]
    public async Task<IActionResult> InviteGuestToDinner(InviteGuestToDinnerRequest request)
    {
        var hostId = _httpContextAccessor.GetHostId();
        var command = _mapper.Map<InviteGuestToDinnerCommand>((request, hostId));

        var createDinnerInvitationResult = await _mediator.Send(command);

        return createDinnerInvitationResult.Match(
            dinner => Ok(_mapper.Map<ReservationResponse>(dinner)),
            errors => Problem(errors));
    }
}