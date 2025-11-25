namespace Users.Domain.Aggregates;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public static bool operator ==(ValueObject? first, ValueObject? second)
    {
        if (first is null && second is null)
        {
            return true;
        }

        if (first is null || second is null)
        {
            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(ValueObject first, ValueObject? second) =>
        !(first == second);

    public bool Equals(ValueObject? other) => other is not null && AreValuesEqual(other);

    public override bool Equals(object? obj) =>
        obj is ValueObject valueOjbect && AreValuesEqual(valueOjbect);

    public override int GetHashCode()
    {
        var hash = GetEqualityComponents()
            .Aggregate(0, (hash, current) => HashCode.Combine(hash, current.GetHashCode()));

        return hash;
    }

    public abstract IEnumerable<object> GetEqualityComponents();

    public bool AreValuesEqual(ValueObject obj) =>
        GetEqualityComponents().SequenceEqual(obj.GetEqualityComponents());
}