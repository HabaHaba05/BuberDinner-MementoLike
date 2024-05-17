using BuberDinner.Domain.AdminAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate;

namespace BuberDinner.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, GuestId? guestId = null, HostId? hostId = null);
    string GenerateToken(Admin admin);
}