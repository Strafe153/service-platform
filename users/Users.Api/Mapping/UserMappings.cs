using Keycloak.AuthServices.Sdk.Admin.Models;
using Users.Api.Application.Commands.Create;
using Users.Api.Application.Queries;
using Users.Domain.Aggregates.User;

namespace Users.Api.Mapping;

public static class UserExtensions
{
    public static UserReadDto ToReadDto(this User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.BirthDate);

    public static UserRepresentation ToKeycloakUser(this CreateUserCommand command) =>
        new()
        {
            Email = command.Email,
            Username = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName
        };
}