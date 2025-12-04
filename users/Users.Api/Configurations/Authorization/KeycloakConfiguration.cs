using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authorization;

namespace Users.Api.Configurations.Authorization;

public static class KeycloakConfigurations
{
    public static void ConfigureKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakWebApiAuthentication(configuration);

        var configSection = configuration.GetSection(ConfigConstants.Keycloak);
        var options = configSection.Get<KeycloakAuthenticationOptions>()!;

        services.Configure<KeycloakAuthenticationOptions>(configSection);

        ConfigureAuthorization(services, configuration, options);
        ConfigureClient(services, configuration);
    }

    private static void ConfigureAuthorization(
        IServiceCollection services,
        IConfiguration configuration,
        KeycloakAuthenticationOptions options)
    {
        services.AddSingleton<IAuthorizationHandler, RequiredRoleOrRequestedUserHandler>();

        services
            .AddAuthorization()
            .AddKeycloakAuthorization(configuration, ConfigConstants.Keycloak)
            .AddAuthorizationBuilder()
            .AddPolicy(KeycloakConstants.Policies.AdminOnly, p =>
            {
                string[] roles = [KeycloakConstants.Roles.Admin];
                p.RequireResourceRolesForClient(options.Resource, roles);
            })
            .AddPolicy(KeycloakConstants.Policies.AdminOrRequestedUser, p =>
            {
                RequiredRoleOrRequestedUserRequirement requirement = new(KeycloakConstants.Roles.Admin);
                p.Requirements.Add(requirement);
            });

        services.AddAuthorizationServer(configuration);
    }

    private static void ConfigureClient(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();

        services
            .AddClientCredentialsTokenManagement()
            .AddClient(ConfigConstants.Keycloak, client =>
            {
                var keycloakOptions = configuration
                    .GetKeycloakOptions<KeycloakAdminClientOptions>(ConfigConstants.KeycloakAdmin)!;

                client.ClientId = ClientId.Parse(keycloakOptions.Resource);
                client.ClientSecret = ClientSecret.Parse(keycloakOptions.Credentials.Secret);
                client.TokenEndpoint = new Uri(keycloakOptions.KeycloakTokenEndpoint);
            });

        services
            .AddKeycloakAdminHttpClient(configuration)
            .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse(ConfigConstants.Keycloak));
    }
}