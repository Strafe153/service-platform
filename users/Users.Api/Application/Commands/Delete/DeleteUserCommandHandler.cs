using System.Diagnostics;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Users.Api.Configurations.Messaging;
using Users.Api.Keycloak;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Delete;

public class DeleteUserCommandHandler(
    IUsersRepository usersRepository,
    IKeycloakClient keycloakClient,
    IPublishEndpoint publishEndpoint,
    IOptions<KeycloakAuthenticationOptions> keycloakOptions,
    ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.delete");

        try
        {
            var user = await usersRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NullReferenceException($"User with id {request.Id} not found.");

            usersRepository.Delete(user);
            await usersRepository.SaveChangesAsync(cancellationToken);

            var userResponse = await keycloakClient.DeleteUserWithResponseAsync(
                keycloakOptions.Value.Realm,
                user.AuthProviderId,
                cancellationToken);

            await userResponse.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);
            await PublishUserDeletedEventAsync(user, cancellationToken);

            logger.LogInformation("Successfully deleter the user with Id={Id}", user.Id);
            UsersTelemetry.UsersDeleted.Add(1);

            activity?
                .SetTag("user.id", user.Id)
                .SetTag("user.keycloak_id", user.AuthProviderId)
                .SetStatus(ActivityStatusCode.Ok);
        }
        catch (DbUpdateException)
        {
            logger.LogError("Failed to delete the user with Id={Id} due to a database failure", request.Id);
            UsersTelemetry.UserDeletionFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Failed to delete the user with Id={Id} due to the exception: {Exception}",
                request.Id,
                ex.Message);

            UsersTelemetry.UserDeletionFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }

    private Task PublishUserDeletedEventAsync(User user, CancellationToken cancellationToken)
    {
        UserDeletedEvent userDeletedEvent = new(user.Email, DateTime.UtcNow);

        var eventTask = publishEndpoint.Publish(
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
