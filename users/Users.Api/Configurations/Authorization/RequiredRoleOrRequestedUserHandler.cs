using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Users.Api.Configurations.Authorization;

public class RequiredRoleOrRequestedUserHandler
    : AuthorizationHandler<RequiredRoleOrRequestedUserRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequiredRoleOrRequestedUserRequirement requirement)
    {
        if (context.User.IsInRole(requirement.Role))
        {
            context.Succeed(requirement);
            return;
        }

        var isSameUser = await IsSameUserRequested(context);

        if (!isSameUser)
        {
            return;
        }

        context.Succeed(requirement);
    }

    private static async Task<bool> IsSameUserRequested(AuthorizationHandlerContext context)
    {
        if (context.Resource is not HttpContext ctx
            || !ctx.Request.RouteValues.ContainsKey(KeycloakConstants.Id))
        {
            return false;
        }

        var requestedId = ctx.Request.RouteValues.GetValueOrDefault(KeycloakConstants.Id);

        if (requestedId is not string id || !Ulid.TryParse(id, out var parsedId))
        {
            return false;
        }

        var identity = (ClaimsIdentity?)context.User.Identity;
        var currentUserIdClaim = identity?.FindFirst(KeycloakConstants.Claims.UserId);

        if (currentUserIdClaim is null || !Ulid.TryParse(currentUserIdClaim.Value, out var currentUserId))
        {
            return false;
        }

        return parsedId == currentUserId;
    }
}