using MediatR;

namespace Users.Api.Application.Commands;

public sealed record IdentifiedCommand<TId, TCommand, TResponse> : IRequest<TResponse>
    where TId : notnull, new()
    where TCommand : class
{
    public TId Id { get; private set; }

    public TCommand Command { get; private set; }

    public IdentifiedCommand(TId id, TCommand command)
    {
        Id = id;
        Command = command;
    }
}