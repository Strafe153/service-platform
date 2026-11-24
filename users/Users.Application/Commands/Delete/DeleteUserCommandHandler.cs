using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Users.Application.Abstractions;
using Users.Application.Constants;
using Users.Application.Exceptions;
using Users.Application.Telemetry;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Application.Commands.Delete;

public abstract class DeleteUserCommandHandler
{
    private readonly IUserService _authService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger _logger;

    protected readonly IUsersRepository _usersRepository;

    public DeleteUserCommandHandler(
        IUsersRepository usersRepository,
        IUserService userService,
        IMessagePublisher messagePublisher,
        ILogger logger)
    {
        _usersRepository = usersRepository;
        _authService = userService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    protected async Task DeleteUserAsync(User user, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.delete");

        try
        {
            _usersRepository.Delete(user);
            await _usersRepository.SaveChangesAsync(cancellationToken);
            
            await _authService.DeleteUserAsync(user.AuthProviderId, cancellationToken);
            await PublishUserDeletedEventAsync(user, cancellationToken);

            _logger.LogInformation("Successfully deleted the user with Id={Id}", user.Id);
            UsersTelemetry.UsersDeleted.Add(1);

            activity?
                .SetTag("user.id", user.Id)
                .SetTag("user.keycloak_id", user.AuthProviderId)
                .SetStatus(ActivityStatusCode.Ok);
        }
        catch (DatabaseException)
        {

            _logger.LogError("Failed to delete the user with Id={Id} due to a database failure", user.Id);
            UsersTelemetry.UserDeletionFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to delete the user with Id={Id} due to the exception: {Exception}",
                user.Id,
                ex.Message);

            UsersTelemetry.UserDeletionFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }

    private Task PublishUserDeletedEventAsync(User user, CancellationToken cancellationToken)
    {
        UserDeletedEvent userDeletedEvent = new(user.Email, DateTime.UtcNow);

        var eventTask = _messagePublisher.Publish(
            userDeletedEvent,
            MessagingKeys.UserDeleted,
            cancellationToken);

        return eventTask;
    }
}