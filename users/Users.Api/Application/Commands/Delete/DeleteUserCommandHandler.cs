using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Users.Api.Configurations.Messaging;
using Users.Api.Keycloak;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Api.Application.Commands.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IKeycloakClient _keycloakClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly KeycloakAuthenticationOptions _keycloakOptions;

    public DeleteUserCommandHandler(
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

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        _usersRepository.Delete(user);
        await _usersRepository.SaveChangesAsync(cancellationToken);

        var userResponse = await _keycloakClient.DeleteUserWithResponseAsync(
            _keycloakOptions.Realm,
            user.AuthProviderId,
            cancellationToken);

        await userResponse.ThrowIfNotSuccessKeycloakStatusCode(cancellationToken);

        UserDeletedEvent userDeletedEvent = new(user.Email, DateTime.UtcNow);

        await _publishEndpoint.Publish(
            userDeletedEvent,
            c =>
            {
                c.CorrelationId = Guid.NewGuid();
                c.SetRoutingKey(RabbitMqConstants.RoutingKeys.UserDeleted);
            },
            cancellationToken);
    }
}