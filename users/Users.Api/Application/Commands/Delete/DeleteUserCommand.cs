using MediatR;

namespace Users.Api.Application.Commands.Delete;

public sealed record DeleteUserCommand(Ulid Id) : IRequest;