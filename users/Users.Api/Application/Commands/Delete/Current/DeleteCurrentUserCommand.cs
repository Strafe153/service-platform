using MediatR;

namespace Users.Api.Application.Commands.Delete.Current;

public sealed record DeleteCurrentUserCommand : IRequest
{
    public static DeleteCurrentUserCommand Instance { get; } = new();
};