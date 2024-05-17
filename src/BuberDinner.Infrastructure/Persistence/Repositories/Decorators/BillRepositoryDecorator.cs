using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BillAggregate;
using BuberDinner.Domain.BillAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;

public class BillRepositoryDecorator : IBillRepository
{
    private readonly IdentityMap _map;
    private readonly IBillRepository _billRepository;
    public BillRepositoryDecorator(IdentityMap map, IBillRepository billRepository)
    {
        _map = map;
        _billRepository = billRepository;
    }

    public async Task AddAsync(Bill bill)
    {
        await _billRepository.AddAsync(bill);
        _map.AddNew(bill);
    }

    public async Task<Bill?> GetByIdAsync(BillId id)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", id.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (Bill)result;
        }

        var becomeHostRequest = await _billRepository.GetByIdAsync(id);
        if (becomeHostRequest is not null)
        {
            _map.AddAlreadyExisting(becomeHostRequest);
        }

        return becomeHostRequest;
    }

    public async Task UpdateAsync(Bill bill)
    {
        await _billRepository.UpdateAsync(bill);
        _map.AddAlreadyExisting(bill);
    }
}