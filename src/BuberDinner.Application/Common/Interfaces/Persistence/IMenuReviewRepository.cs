using BuberDinner.Domain.MenuReviewAggregate;

namespace BuberDinner.Application.Common.Interfaces.Persistence;
public interface IMenuReviewRepository
{
    Task AddAsync(MenuReview menuReview);
}