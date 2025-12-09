using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Users.Domain.Aggregates.User;

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

        var hasSubClaim = context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier);

        if (!hasSubClaim)
        {
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

        // Identity cannot be null at this point due to the previous checks
        var nameClaim = ((ClaimsIdentity)context.User.Identity!).FindFirst(ClaimTypes.NameIdentifier);
        
        if (nameClaim is null)
        {
            return false;
        }

        var requestedId = ctx.Request.RouteValues.GetValueOrDefault(KeycloakConstants.Id);

        if (requestedId is not string id || !Ulid.TryParse(id, out var parsedId))
        {
            return false;
        }

        using var scope = ctx.RequestServices.CreateScope();
        var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var user = await usersRepository.GetByAuthProviderIdAsync(nameClaim.Value, CancellationToken.None);

        return parsedId == user?.Id;
    }
}