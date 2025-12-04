using System.Text.Json.Serialization;

namespace Users.Api.Keycloak;

public sealed class KeycloakErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = default!;
}
