using System.Text.Json.Serialization;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Users.Api.Application.Queries;
using Users.Api.Extensions;
using Users.Api.Mapping;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Create;

public sealed class KeycloakErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = default!;
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserReadDto>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IKeycloakClient _keycloakClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly KeycloakAuthenticationOptions _keycloakOptions;

    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IKeycloakClient keycloakClient,
        IPublishEndpoint publishEndpoint,
        IOptions<KeycloakAuthenticationOptions> keycloakOptions)
    {
        _usersRepository = usersRepository;
        _keycloakClient = keycloakClient;
        _publishEndpoint = publishEndpoint;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async Task<UserReadDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = string.Empty;
        
        try
        {
            var keycloakUser = request.ToKeycloakUser();
            keycloakUser.Enabled = true;

            var userResponse = await _keycloakClient.CreateUserWithResponseAsync(
                _keycloakOptions.Realm,
                keycloakUser,
                cancellationToken);

            await userResponse.ThrowIfNotSuccessKeycloakStatusCode(cancellationToken);

            if (userResponse.Headers.TryGetValues(HeaderNames.Location, out var locationHeaders))
            {
                userId = locationHeaders.First().Split('/')[^1];
                await SetUserPasswordAsync(request, userId, cancellationToken);

                var user = await SaveUserAsync(request, userId, cancellationToken);
                await PublishUserCreatedEvent(request, cancellationToken);

                return user.ToReadDto();
            }

            // check what the response looks like
            throw new ArgumentNullException(userId, "No location header was returned from Keycloak.");
        }
        catch (DbUpdateException)
        {
            await _keycloakClient.DeleteUserAsync(_keycloakOptions.Realm, userId, cancellationToken);
            throw;
        }
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

        await response.ThrowIfNotSuccessKeycloakStatusCode(cancellationToken);
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

    private Task PublishUserCreatedEvent(CreateUserCommand command, CancellationToken cancellationToken)
    {
        UserCreatedEvent userCreatedEvent = new(command.Email, DateTime.UtcNow);

        return _publishEndpoint.Publish(
            userCreatedEvent,
            c => c.CorrelationId = Guid.NewGuid(),
            cancellationToken);
    }
}