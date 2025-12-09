namespace Users.Domain.Events;

public sealed record UserDeletedEvent(string Email, DateTime DeletedAt);