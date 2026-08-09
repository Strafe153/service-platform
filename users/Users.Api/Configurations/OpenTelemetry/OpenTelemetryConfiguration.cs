using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Users.Api.Configurations.OpenTelemetry;

public static class OpenTelemetryConfiguration
{
    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(ConfigConstants.OpenTelemetry).Get<OpenTelemetryOptions>()!;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(OpenTelemetryOptions.ServiceName, serviceVersion: "1.0.0")
            .AddAttributes([
                new KeyValuePair<string, object>("environment", environment),
                new KeyValuePair<string, object>("host", Environment.MachineName)
            ]);

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.AddOtlpExporter(ConfigureExporter);
        });

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("System.Net.Http")
                .AddMeter("System.Net.NameResolution")
                .AddMeter(OpenTelemetryOptions.ServiceName)
                .AddOtlpExporter(ConfigureExporter))
            .WithTracing(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddSource(OpenTelemetryOptions.ServiceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(ConfigureExporter));

        void ConfigureExporter(OtlpExporterOptions exporterOptions)
        {
            exporterOptions.Endpoint = new Uri(options.Endpoint);
            exporterOptions.Protocol = OtlpExportProtocol.Grpc;
        }
    }
}
