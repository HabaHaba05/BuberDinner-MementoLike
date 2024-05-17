using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;

namespace BuberDinner.Application.Common.Interfaces.Persistence;
public interface IBecomeHostRequestRepository
{
    Task UpdateAsync(BecomeHostRequest becomeHostRequest);
    Task AddAsync(BecomeHostRequest becomeHostRequest);
    Task<BecomeHostRequest?> GetByIdAsync(BecomeHostRequestId becomeHostRequestId);
    Task<List<BecomeHostRequest>> GetByUserIdAsync(UserId userId);
    Task<List<BecomeHostRequest>> ListPendingRequestsAsync();
}