using BuberDinner.Domain.BillAggregate;
using BuberDinner.Domain.BillAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;
public interface IBillRepository
{
    Task<Bill?> GetByIdAsync(BillId id);
    Task UpdateAsync(Bill bill);
    Task AddAsync(Bill bill);
}