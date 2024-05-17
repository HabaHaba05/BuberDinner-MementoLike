using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Domain.AdminAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuberDinner.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateToken(User user, GuestId? guestId = null, HostId? hostId = null)
    {
        List<Claim> claims =
        [
            new Claim(Constants.Authentication.ClaimTypes.FirstName, user.FirstName),
            new Claim(Constants.Authentication.ClaimTypes.LastName, user.LastName),
            new Claim(Constants.Authentication.ClaimTypes.UserId, user.Id.Value.ToString())
        ];

        if (guestId is not null)
        {
            claims.Add(new Claim(Constants.Authentication.ClaimTypes.GuestId, guestId.Value.ToString()));
        }

        if (hostId is not null)
        {
            claims.Add(new Claim(Constants.Authentication.ClaimTypes.HostId, hostId.Value.ToString()));
        }

        return GenerateToken(claims);
    }

    public string GenerateToken(Admin admin)
    {
        return GenerateToken([
            new Claim(Constants.Authentication.ClaimTypes.Name, admin.Name),
            new Claim(Constants.Authentication.ClaimTypes.AdminId, admin.Id.Value.ToString()),
        ]);
    }

    private string GenerateToken(List<Claim> claimsToAdd)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            .. claimsToAdd,
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}