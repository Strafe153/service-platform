using MediatR;

namespace Users.Api.Application.Commands.Delete.ById;

public sealed record DeleteUserByIdCommand(Guid Id) : IRequest;