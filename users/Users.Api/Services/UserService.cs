using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Users.Api.Keycloak;
using Users.Application.Abstractions;
using Users.Application.Models;

namespace Users.Api.Services;

internal class UserService : IUserService
{
    private readonly IKeycloakClient _client;
    private readonly KeycloakAuthenticationOptions _options;

    public UserService(
        IKeycloakClient client,
        IOptions<KeycloakAuthenticationOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<string> CreateUserAsync(
        UserRequest request,
        CancellationToken cancellationToken)
    {
        UserRepresentation user = new()
        {
            Email = request.Email,
            Username = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Enabled = true,
        };

        var response = await _client.CreateUserWithResponseAsync(
            _options.Realm,
            user,
            cancellationToken);

        await response.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);

        if (!response.Headers.TryGetValues(HeaderNames.Location, out var locationHeaders))
        {
            throw new InvalidOperationException("No 'Location' header was returned from Keycloak.");
        }

        var id = locationHeaders.First().Split('/')[^1];

        return id;
    }

    public async Task ResetPasswordAsync(
        string userId,
        string password,
        CancellationToken cancellationToken)
    {
        CredentialRepresentation representation = new()
        {
            Value = password
        };

        var response = await _client.ResetPasswordWithResponseAsync(
            _options.Realm,
            userId,
            representation,
            cancellationToken);

        await response.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var response = await _client.DeleteUserWithResponseAsync(_options.Realm, userId, cancellationToken);
        await response.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);
    }
}