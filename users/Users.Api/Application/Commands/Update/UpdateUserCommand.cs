using MediatR;

namespace Users.Api.Application.Commands.Update;

public sealed record UpdateUserCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateOnly BirthDate) : IRequest;