namespace Users.Api.Configurations.Authorization;

public static class KeycloakConstants
{
    public const string Id = "id";

    public static class Roles
    {
        public const string Admin = "admin";
    }

    public static class Policies
    {
        public const string AdminOnly = "admin-only";
    }
}