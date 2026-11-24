namespace Users.Api.Keycloak;

public static class HttpResponsemessageExtensions
{
    public static async Task ThrowIfNotSuccessKeycloakStatusCodeAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var keycloakResult = await response.Content
                .ReadFromJsonAsync<KeycloakErrorResponse>(cancellationToken);

            throw new InvalidOperationException(keycloakResult?.Error ?? "Keycloak operation failed.");
        }
    }
}