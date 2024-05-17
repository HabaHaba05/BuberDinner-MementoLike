using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.BecomeHostRequestAggregate;
using BuberDinner.Domain.BecomeHostRequestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class BecomeHostRequestRepositoryDecorator : IBecomeHostRequestRepository
{
    private readonly IdentityMap _map;
    private readonly IBecomeHostRequestRepository _becomeHostRequestRepository;
    public BecomeHostRequestRepositoryDecorator(IdentityMap map, IBecomeHostRequestRepository becomeHostRequestRepository)
    {
        _map = map;
        _becomeHostRequestRepository = becomeHostRequestRepository;
    }

    public async Task AddAsync(BecomeHostRequest becomeHostRequest)
    {
        await _becomeHostRequestRepository.AddAsync(becomeHostRequest);
        _map.AddNew(becomeHostRequest);
    }

    public async Task<BecomeHostRequest?> GetByIdAsync(BecomeHostRequestId becomeHostRequestId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", becomeHostRequestId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (BecomeHostRequest)result;
        }

        var becomeHostRequest = await _becomeHostRequestRepository.GetByIdAsync(becomeHostRequestId);
        if (becomeHostRequest is not null)
        {
            _map.AddAlreadyExisting(becomeHostRequest);
        }

        return becomeHostRequest;
    }

    public async Task<List<BecomeHostRequest>> GetByUserIdAsync(UserId userId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("UserId", userId.Value)]);

        if (result is not null || result.Any())
        {
            return result.Select(x => (BecomeHostRequest)x).ToList();
        }

        var becomeHostRequests = await _becomeHostRequestRepository.GetByUserIdAsync(userId);
        becomeHostRequests.ForEach(x => _map.AddAlreadyExisting(x));

        return becomeHostRequests;
    }

    public async Task<List<BecomeHostRequest>> ListPendingRequestsAsync()
    {
        var requests = await _becomeHostRequestRepository.ListPendingRequestsAsync();
        requests.ForEach(x => _map.AddAlreadyExisting(x));
        return requests;
    }

    public async Task UpdateAsync(BecomeHostRequest becomeHostRequest)
    {
        await _becomeHostRequestRepository.UpdateAsync(becomeHostRequest);
        _map.AddAlreadyExisting(becomeHostRequest);
    }
}