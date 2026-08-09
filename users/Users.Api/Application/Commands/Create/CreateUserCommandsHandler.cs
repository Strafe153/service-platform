using System.Diagnostics;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Users.Api.Application.Queries.Dto;
using Users.Api.Configurations.Messaging;
using Users.Api.Keycloak;
using Users.Api.Mapping;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Create;

public sealed class CreateUserCommandHandler(
    IUsersRepository usersRepository,
    IPublishEndpoint publishEndpoint,
    IKeycloakClient keycloakClient,
    IOptions<KeycloakAuthenticationOptions> keycloakOptions,
    ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, UserReadDto>
{
    public async Task<UserReadDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.create");

        var userId = string.Empty;

        try
        {
            var keycloakUser = await CreateUserAsync(request, cancellationToken);
            userId = keycloakUser.Id!;

            await SetUserPasswordAsync(request, userId, cancellationToken);
            var dbUser = await SaveUserAsync(request, userId, cancellationToken);

            var updateTask = SetUserIdAttributeAsync(keycloakUser, dbUser.Id, cancellationToken);
            var eventTask = PublishUserCreatedEventAsync(request, cancellationToken);

            await Task.WhenAll(updateTask, eventTask);

            logger.LogInformation("Successfully created a user");
            UsersTelemetry.UsersCreated.Add(1);

            activity?
                .SetTag("user.keycloak_id", userId)
                .SetTag("user.id", dbUser.Id)
                .SetStatus(ActivityStatusCode.Ok);

            return dbUser.ToReadDto();
        }
        catch (DbUpdateException)
        {
            logger.LogError("Failed to create a user due to a database failure");
            UsersTelemetry.UserCreationFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            await keycloakClient.DeleteUserAsync(keycloakOptions.Value.Realm, userId, cancellationToken);

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create a user due to the exception: {Exception}", ex.Message);
            UsersTelemetry.UserCreationFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }

    private async Task<UserRepresentation> CreateUserAsync(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var keycloakUser = request.ToKeycloakUser();
        keycloakUser.Enabled = true;

        var userResponse = await keycloakClient.CreateUserWithResponseAsync(
            keycloakOptions.Value.Realm,
            keycloakUser,
            cancellationToken);

        await userResponse.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);

        if (!userResponse.Headers.TryGetValues(HeaderNames.Location, out var locationHeaders))
        {
            throw new ArgumentException("No 'Location' header was returned from Keycloak.");
        }

        keycloakUser.Id = locationHeaders.First().Split('/')[^1];

        return keycloakUser;
    }

    private async Task SetUserPasswordAsync(
        CreateUserCommand command,
        string userId,
        CancellationToken cancellationToken)
    {
        CredentialRepresentation credentials = new()
        {
            Value = command.Password
        };

        var response = await keycloakClient.ResetPasswordWithResponseAsync(
            keycloakOptions.Value.Realm,
            userId,
            credentials,
            cancellationToken);

        await response.ThrowIfNotSuccessKeycloakStatusCodeAsync(cancellationToken);
    }

    private async Task<User> SaveUserAsync(
        CreateUserCommand command,
        string userId,
        CancellationToken cancellationToken)
    {
        User user = new(
            command.Email,
            command.FirstName,
            command.LastName,
            command.PhoneNumber,
            command.BirthDate,
            userId,
            command.Address);

        usersRepository.Add(user);
        await usersRepository.SaveChangesAsync(cancellationToken);

        return user;
    }

    private Task<HttpResponseMessage> SetUserIdAttributeAsync(
        UserRepresentation keycloakUser,
        Guid dbUserId,
        CancellationToken cancellationToken)
    {
        keycloakUser.Attributes = new Dictionary<string, ICollection<string>>()
        {
            { "user_id", [dbUserId.ToString()] }
        };

        var updateTask = keycloakClient.UpdateUserWithResponseAsync(
            keycloakOptions.Value.Realm,
            keycloakUser.Id!,
            keycloakUser,
            cancellationToken);

        return updateTask;
    }

    private Task PublishUserCreatedEventAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        UserCreatedEvent userCreatedEvent = new(command.Email, DateTime.UtcNow);

        var eventTask = publishEndpoint.Publish(
            userCreatedEvent,
            c =>
            {
                c.CorrelationId = Guid.NewGuid();
                c.SetRoutingKey(RabbitMqConstants.RoutingKeys.UserCreated);
            },
            cancellationToken);

        return eventTask;
    }
}
