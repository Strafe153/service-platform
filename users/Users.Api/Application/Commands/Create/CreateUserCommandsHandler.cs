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
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserReadDto>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IKeycloakClient _keycloakClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly KeycloakAuthenticationOptions _keycloakOptions;

    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPublishEndpoint publishEndpoint,
        IKeycloakClient keycloakClient,
        IOptions<KeycloakAuthenticationOptions> keycloakOptions)
    {
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
        _keycloakClient = keycloakClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async Task<UserReadDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
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

            return dbUser.ToReadDto();
        }
        catch (DbUpdateException)
        {
            await _keycloakClient.DeleteUserAsync(_keycloakOptions.Realm, userId, cancellationToken);
            throw;
        }
    }

    private async Task<UserRepresentation> CreateUserAsync(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var keycloakUser = request.ToKeycloakUser();
        keycloakUser.Enabled = true;

        var userResponse = await _keycloakClient.CreateUserWithResponseAsync(
            _keycloakOptions.Realm,
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
        
        var response = await _keycloakClient.ResetPasswordWithResponseAsync(
            _keycloakOptions.Realm,
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

        _usersRepository.Add(user);
        await _usersRepository.SaveChangesAsync(cancellationToken);

        return user;
    }

    private Task<HttpResponseMessage> SetUserIdAttributeAsync(
        UserRepresentation keycloakUser,
        Ulid dbUserId,
        CancellationToken cancellationToken)
    {
        keycloakUser.Attributes = new Dictionary<string, ICollection<string>>()
        {
            { "user_id", [dbUserId.ToString()] }
        };

        var updateTask = _keycloakClient.UpdateUserWithResponseAsync(
            _keycloakOptions.Realm,
            keycloakUser.Id!,
            keycloakUser,
            cancellationToken);

        return updateTask;
    }

    private Task PublishUserCreatedEventAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        UserCreatedEvent userCreatedEvent = new(command.Email, DateTime.UtcNow);

        var eventTask = _publishEndpoint.Publish(
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