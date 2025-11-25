namespace Users.Api.Application.Queries;

public sealed record UserReadDto(
    Ulid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateOnly BirthDate
);