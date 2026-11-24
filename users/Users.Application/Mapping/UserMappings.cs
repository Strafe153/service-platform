using Users.Application.Commands.Create;
using Users.Application.Models;
using Users.Application.Queries.Dto;
using Users.Domain.Aggregates.User;

namespace Users.Application.Mapping;

public static class UserExtensions
{
    public static UserReadDto ToReadDto(this User user)
    {
        AddressReadDto address = new(
            user.Address.Country,
            user.Address.State,
            user.Address.City,
            user.Address.ZipCode,
            user.Address.Street
        );

        UserReadDto dto = new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.BirthDate,
            address);

        return dto;
    }

    public static User ToUser(this CreateUserCommand command, string authProviderId) => new(
        command.Email,
        command.FirstName,
        command.LastName,
        command.PhoneNumber,
        command.BirthDate,
        authProviderId,
        command.Address);

    public static UserRequest ToUserRequest(this CreateUserCommand command) =>
        new(command.Email, command.FirstName, command.LastName);
}
