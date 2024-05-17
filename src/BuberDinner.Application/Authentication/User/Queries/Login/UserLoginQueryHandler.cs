using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Authentication.User.Queries.Login;

public class UserLoginQueryHandler :
    IRequestHandler<UserLoginQuery, ErrorOr<UserAuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public UserLoginQueryHandler(
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserAuthenticationResult>> Handle(UserLoginQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (await _unitOfWork.UserRepository.GetUserByEmailAsync(query.Email) is not Domain.UserAggregate.User user)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        if (user.Password != query.Password)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var guestId = await _unitOfWork.GuestRepository.GetGuestIdOfUserAsync(user.Id);
        var hostId = await _unitOfWork.HostRepository.GetHostIdOfUserAsync(user.Id);

        var token = _jwtTokenGenerator.GenerateToken(user, guestId, hostId);

        return new UserAuthenticationResult(
            user,
            token);
    }
}