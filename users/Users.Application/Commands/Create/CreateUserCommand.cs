using MediatR;
using Users.Application.Queries.Dto;
using Users.Domain.Aggregates.User;

namespace Users.Application.Commands.Create;

public sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateOnly BirthDate,
    string Password,
    Address Address) : IRequest<UserReadDto>;