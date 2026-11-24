using MediatR;
using Microsoft.Extensions.Logging;
using Users.Application.Abstractions;
using Users.Domain.Aggregates.User;

namespace Users.Application.Commands.Delete.Current;

public sealed class DeleteCurrentUserCommandHandler
    : DeleteUserCommandHandler, IRequestHandler<DeleteCurrentUserCommand>
{
    private readonly ICurrentUserService _currentUser;

    public DeleteCurrentUserCommandHandler(
        ICurrentUserService currentUserService,
        IUsersRepository usersRepository,
        IUserService userService,
        IMessagePublisher messagePublisher,
        ILogger<DeleteCurrentUserCommandHandler> logger)
            : base(usersRepository, userService, messagePublisher, logger)
    {
        _currentUser = currentUserService;
    }

    public async Task Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var authProviderId = _currentUser.GetId();

        var user = await _usersRepository.GetByAuthProviderIdAsync(authProviderId, cancellationToken)
            ?? throw new NullReferenceException($"User with id auth provider id {authProviderId} not found.");

        await DeleteUserAsync(user, cancellationToken);
    }
}