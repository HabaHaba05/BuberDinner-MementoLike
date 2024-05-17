using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Common.DomainErrors;

using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Authentication.Admin.Queries.Login;

public class AdminLoginQueryHandler :
    IRequestHandler<AdminLoginQuery, ErrorOr<AdminAuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AdminLoginQueryHandler(
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AdminAuthenticationResult>> Handle(AdminLoginQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        // 1. Validate the user exists
        if (await _unitOfWork.AdminRepository.GetAdminByEmailAsync(query.Email) is not Domain.AdminAggregate.Admin admin)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        // 2. Validate the password is correct
        if (admin.Password != query.Password)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        // 3. Create JWT token
        var token = _jwtTokenGenerator.GenerateToken(admin);

        return new AdminAuthenticationResult(
            admin,
            token);
    }
}