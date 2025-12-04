using Keycloak.AuthServices.Common;

namespace Users.Api.Keycloak;

public static class HttpResponsemessageExtensions
{
    public static async Task ThrowIfNotSuccessKeycloakStatusCode(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var keycloakResult = await response.Content
                .ReadFromJsonAsync<KeycloakErrorResponse>(cancellationToken);

            throw new KeycloakException(keycloakResult?.Error ?? "Keycloak operation failed.");
        }
    }
}