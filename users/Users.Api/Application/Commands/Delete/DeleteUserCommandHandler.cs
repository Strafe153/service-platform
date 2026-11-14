using System.Diagnostics;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Users.Api.Configurations.Messaging;
using Users.Api.Keycloak;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Delete;

public abstract class DeleteUserCommandHandler
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOptions<KeycloakAuthenticationOptions> _keycloakOptions;
    private readonly ILogger _logger;

    protected readonly IUsersRepository _usersRepository;

    public DeleteUserCommandHandler(
        IUsersRepository usersRepository,
        IKeycloakClient keycloakClient,
        IPublishEndpoint publishEndpoint,
        IOptions<KeycloakAuthenticationOptions> keycloakOptions,
        ILogger logger)
    {
        _usersRepository = usersRepository;
        _keycloakClient = keycloakClient;
        _publishEndpoint = publishEndpoint;
        _keycloakOptions = keycloakOptions;
        _logger = logger;
    }

    protected async Task DeleteUserAsync(User user, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.delete");

        try
        {
            _usersRepository.Delete(user);
            await _usersRepository.SaveChangesAsync(cancellationToken);

            var userResponse = await _keycloakClient.DeleteUserWithResponseAsync(
                _keycloakOptions.Value.Realm,
                user.AuthProviderId,
                cancellationToken);

            await userResponse.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);
            await PublishUserDeletedEventAsync(user, cancellationToken);

            _logger.LogInformation("Successfully deleted the user with Id={Id}", user.Id);
            UsersTelemetry.UsersDeleted.Add(1);

            activity?
                .SetTag("user.id", user.Id)
                .SetTag("user.keycloak_id", user.AuthProviderId)
                .SetStatus(ActivityStatusCode.Ok);
        }
        catch (DbUpdateException)
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

        var eventTask = _publishEndpoint.Publish(
            userDeletedEvent,
            c =>
            {
                c.CorrelationId = Guid.NewGuid();
                c.SetRoutingKey(RabbitMqConstants.RoutingKeys.UserDeleted);
            },
            cancellationToken);

        return eventTask;
    }
}