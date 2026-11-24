using System.Security.Claims;
using Users.Application.Abstractions;

namespace Users.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public string GetId()
    {
        var authProviderId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(authProviderId))
        {
            throw new NullReferenceException("No auth provider id passed.");
        }

        return authProviderId;
    }
}