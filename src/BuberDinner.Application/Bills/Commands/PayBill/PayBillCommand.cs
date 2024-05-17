using BuberDinner.Domain.BillAggregate;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Bills.Commands.PayBill;
public record PayBillCommand(string BillId) : IRequest<ErrorOr<Bill>>;