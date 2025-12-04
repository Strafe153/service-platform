using Microsoft.AspNetCore.Authorization;

namespace Users.Api.Configurations.Authorization;

public class RequiredRoleOrRequestedUserRequirement(string role) : IAuthorizationRequirement
{
    public string Role => role;
}