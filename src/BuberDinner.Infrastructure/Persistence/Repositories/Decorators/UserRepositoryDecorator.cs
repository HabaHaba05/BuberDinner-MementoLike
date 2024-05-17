using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.UserAggregate;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

namespace BuberDinner.Infrastructure.Persistence.Repositories.Decorators;

public class UserRepositoryDecorator : IUserRepository
{
    private readonly IdentityMap _map;
    private readonly IUserRepository _userRepository;
    public UserRepositoryDecorator(IdentityMap map, IUserRepository userRepository)
    {
        _map = map;
        _userRepository = userRepository;
    }

    public async Task AddAsync(User user)
    {
        await _userRepository.AddAsync(user);
        _map.AddNew(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Email", email)]).FirstOrDefault();

        if (result is not null)
        {
            return (User)result;
        }

        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user is not null)
        {
            _map.AddAlreadyExisting(user);
        }

        return user;
    }

    public async Task<User?> GetUserByIdAsync(UserId userId)
    {
        var result = _map.FindByProperties([new KeyValuePair<string, object?>("Id", userId.Value)]).FirstOrDefault();

        if (result is not null)
        {
            return (User)result;
        }

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is not null)
        {
            _map.AddAlreadyExisting(user);
        }

        return user;
    }
}