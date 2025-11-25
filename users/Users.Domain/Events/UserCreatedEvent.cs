namespace Users.Domain.Events;

public sealed record UserCreatedEvent(string Email, DateTime CreatedAt);