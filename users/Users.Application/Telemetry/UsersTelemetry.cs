using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Users.Application.Telemetry;

public static class UsersTelemetry
{
    public const string ServiceName = "Users.Api";

    public static ActivitySource ActivitySource { get; } = new(ServiceName);

    public static Meter Meter { get; } = new(ServiceName);

    public static Counter<int> UsersCreated { get; } = Meter.CreateCounter<int>("users.created");

    public static Counter<int> UserCreationFailures { get; } =
        Meter.CreateCounter<int>("users.creation.failures");

    public static Counter<int> UsersUpdated { get; } = Meter.CreateCounter<int>("users.updated");

    public static Counter<int> UserUpdateFailures { get; } =
        Meter.CreateCounter<int>("users.update.failures");

    public static Counter<int> UsersDeleted { get; } = Meter.CreateCounter<int>("users.deleted");

    public static Counter<int> UserDeletionFailures { get; } =
        Meter.CreateCounter<int>("users.deletion.failures");

    public static Counter<int> UserAddressUpdated { get; } =
        Meter.CreateCounter<int>("users.address.updated");

    public static Counter<int> UserAddressUpdateFailures { get; } =
        Meter.CreateCounter<int>("users.address.update.failures");
}
