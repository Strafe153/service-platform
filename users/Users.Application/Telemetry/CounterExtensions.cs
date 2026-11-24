using System.Diagnostics.Metrics;
using System.Numerics;

namespace Users.Application.Telemetry;

public static class CounterExtensions
{
    public static void IncrementError<T>(
        this Counter<T> counter,
        string tagKey = "failure_reason",
        string tagValue = "unknown") where T : struct, INumber<T> =>
            counter.Add(T.One, new KeyValuePair<string, object?>(tagKey, tagValue));
}
