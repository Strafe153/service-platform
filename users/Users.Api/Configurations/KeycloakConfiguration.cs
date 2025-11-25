using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;

namespace Users.Api.Configurations;

public static class KeycloakConfigurations
{
    public static void ConfigureKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakWebApiAuthentication(configuration);

        var configSection = configuration.GetSection(ConfigConstants.Keycloak);
        var options = configSection.Get<KeycloakAuthenticationOptions>()!;

        services.Configure<KeycloakAuthenticationOptions>(configSection);

        services
            .AddAuthorization()
            .AddKeycloakAuthorization(configuration, ConfigConstants.KeycloakAdmin)
            .AddAuthorizationBuilder()
            .AddPolicy("admin-only", p => p.RequireResourceRolesForClient(options.Resource, ["admin"]));

        services.AddAuthorizationServer(configuration);

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