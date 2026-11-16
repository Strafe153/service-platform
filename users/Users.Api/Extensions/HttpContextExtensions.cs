using System.Security.Claims;

namespace Users.Api.Extensions;

public static class HttpContextExtensions
{
    extension(IHttpContextAccessor contextAccessor)
    {
        public string GetAuthProviderId()
        {
            var authProviderId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (authProviderId is null || string.IsNullOrEmpty(authProviderId.Value))
            {
                throw new NullReferenceException("No auth provider id passed.");
            }

            return authProviderId.Value;
        }
    }
}
