namespace Users.Api.Application.Queries.Dto;

public sealed record AddressReadDto(
    string Country,
    string State,
    string City,
    string ZipCode,
    string? Street);