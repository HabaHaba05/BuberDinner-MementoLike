using BuberDinner.Application.Bills.Commands.PayBill;
using BuberDinner.Contracts.Bills;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("bills")]
[Authorize]
public class BillsController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;
    public BillsController(IMapper mapper, ISender mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> PayBill(PayBillRequest request)
    {
        var payBillResult = await _mediator.Send(new PayBillCommand(request.BillId));

        return payBillResult.Match(
            dinner => Ok(_mapper.Map<BillResponse>(dinner)),
            errors => Problem(errors));
    }
}