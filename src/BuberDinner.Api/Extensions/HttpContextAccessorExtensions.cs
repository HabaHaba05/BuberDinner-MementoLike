using BuberDinner.Domain.AdminAggregate.ValueObjects;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure;

namespace BuberDinner.Api.Extensions;

public static class HttpContextAccessorExtensions
{
    public static UserId? GetUserId(this IHttpContextAccessor context)
    {
        var idStr = context.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == Constants.Authentication.ClaimTypes.UserId)?.Value ?? string.Empty;
        return string.IsNullOrEmpty(idStr) ? null : UserId.Create(Guid.Parse(idStr));
    }

    public static AdminId? GetAdminId(this IHttpContextAccessor context)
    {
        var idStr = context.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == Constants.Authentication.ClaimTypes.AdminId)?.Value ?? string.Empty;
        return string.IsNullOrEmpty(idStr) ? null : AdminId.Create(Guid.Parse(idStr));
    }

    public static HostId? GetHostId(this IHttpContextAccessor context)
    {
        var idStr = context.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == Constants.Authentication.ClaimTypes.HostId)?.Value ?? string.Empty;
        return string.IsNullOrEmpty(idStr) ? null : HostId.Create(Guid.Parse(idStr));
    }

    public static GuestId? GetGuestId(this IHttpContextAccessor context)
    {
        var idStr = context.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == Constants.Authentication.ClaimTypes.GuestId)?.Value ?? string.Empty;
        return string.IsNullOrEmpty(idStr) ? null : GuestId.Create(Guid.Parse(idStr));
    }
}