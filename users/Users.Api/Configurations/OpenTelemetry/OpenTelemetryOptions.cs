namespace Users.Api.Configurations.OpenTelemetry;

public class OpenTelemetryOptions
{
    public const string ServiceName = "Users.Api";

    public required string Endpoint { get; set; }
}
