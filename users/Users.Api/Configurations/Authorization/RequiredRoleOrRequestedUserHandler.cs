using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Users.Api.Configurations.Authorization;

public class RequiredRoleOrRequestedUserHandler
    : AuthorizationHandler<RequiredRoleOrRequestedUserRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequiredRoleOrRequestedUserRequirement requirement)
    {
        if (context.User.IsInRole(requirement.Role) || IsSameUserRequested(context))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool IsSameUserRequested(AuthorizationHandlerContext context)
    {
        if (context.Resource is not HttpContext ctx
            || !ctx.Request.RouteValues.ContainsKey(KeycloakConstants.Id))
        {
            return false;
        }

        var requestedId = ctx.Request.RouteValues.GetValueOrDefault(KeycloakConstants.Id);

        if (requestedId is not string id || !Guid.TryParse(id, out var parsedId))
        {
            return false;
        }

        var identity = (ClaimsIdentity?)context.User.Identity;
        var currentUserIdClaim = identity?.FindFirst(KeycloakConstants.Claims.UserId);

        if (currentUserIdClaim is null || !Guid.TryParse(currentUserIdClaim.Value, out var currentUserId))
        {
            return false;
        }

        return parsedId == currentUserId;
    }
}