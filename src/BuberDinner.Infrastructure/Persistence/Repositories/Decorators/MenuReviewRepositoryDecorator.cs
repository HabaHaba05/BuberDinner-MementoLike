using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.MenuReviewAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
public class MenuReviewRepositoryDecorator : IMenuReviewRepository
{
    private readonly IdentityMap _map;
    private readonly IMenuReviewRepository _menuReviewRepository;
    public MenuReviewRepositoryDecorator(IdentityMap map, IMenuReviewRepository menuReviewRepository)
    {
        _map = map;
        _menuReviewRepository = menuReviewRepository;
    }

    public async Task AddAsync(MenuReview menuReview)
    {
        await _menuReviewRepository.AddAsync(menuReview);
        _map.AddNew(menuReview);
    }
}