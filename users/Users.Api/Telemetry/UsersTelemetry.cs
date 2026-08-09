using System.Diagnostics;
using System.Diagnostics.Metrics;
using Users.Api.Configurations.OpenTelemetry;

namespace Users.Api.Telemetry;

public static class UsersTelemetry
{
    public static ActivitySource ActivitySource { get; } = new(OpenTelemetryOptions.ServiceName);

    public static Meter Meter { get; } = new(OpenTelemetryOptions.ServiceName);

    public static Counter<long> UsersCreated { get; } = Meter.CreateCounter<long>("users.created");

    public static Counter<long> UserCreationFailures { get; } =
        Meter.CreateCounter<long>("users.creation.failures");

    public static Counter<long> UsersUpdated { get; } = Meter.CreateCounter<long>("users.updated");

    public static Counter<long> UserUpdateFailures { get; } =
        Meter.CreateCounter<long>("users.update.failures");

    public static Counter<long> UsersDeleted { get; } = Meter.CreateCounter<long>("users.deleted");

    public static Counter<long> UserDeletionFailures { get; } =
        Meter.CreateCounter<long>("users.deletion.failures");

    public static Counter<long> UserAddressUpdated { get; } =
        Meter.CreateCounter<long>("users.address.updated");

    public static Counter<long> UserAddressUpdateFailures { get; } =
        Meter.CreateCounter<long>("users.address.update.failures");
}
