using MediatR;

namespace Users.Application.Commands.Delete.ById;

public sealed record DeleteUserByIdCommand(Guid Id) : IRequest;