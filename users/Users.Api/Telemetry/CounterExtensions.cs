using System.Diagnostics.Metrics;
using System.Numerics;

namespace Users.Api.Telemetry;

public static class CounterExtensions
{
    extension<T>(Counter<T> counter) where T : struct, INumber<T>
    {
        public void IncrementError(string tagKey = "failure_reason", string tagValue = "unknown") =>
            counter.Add(T.One, new KeyValuePair<string, object?>(tagKey, tagValue));
    }
}
