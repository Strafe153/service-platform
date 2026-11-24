using MediatR;
using Microsoft.Extensions.Logging;
using Users.Application.Abstractions;
using Users.Domain.Aggregates.User;

namespace Users.Application.Commands.Delete.ById;

public sealed class DeleteUserByIdCommandHandler(
    IUsersRepository usersRepository,
    IUserService userService,
    IMessagePublisher messagePublisher,
    ILogger<DeleteUserByIdCommandHandler> logger)
        : DeleteUserCommandHandler(usersRepository, userService, messagePublisher, logger),
        IRequestHandler<DeleteUserByIdCommand>
{
    public async Task Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        await DeleteUserAsync(user, cancellationToken);
    }
}
