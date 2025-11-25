namespace Users.Domain.Aggregates;

public class Entity<T>(T id) where T : notnull, new()
{
    public T Id { get; private set; } = id;
}