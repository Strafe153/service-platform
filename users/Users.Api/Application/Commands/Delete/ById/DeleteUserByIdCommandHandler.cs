using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.Delete.ById;

public sealed class DeleteUserByIdCommandHandler
    : DeleteUserCommandHandler, IRequestHandler<DeleteUserByIdCommand>
{
    public DeleteUserByIdCommandHandler(
        IUsersRepository usersRepository,
        IKeycloakClient keycloakClient,
        IPublishEndpoint publishEndpoint,
        IOptions<KeycloakAuthenticationOptions> keycloakOptions,
        ILogger<DeleteUserByIdCommandHandler> logger)
            : base(usersRepository, keycloakClient, publishEndpoint, keycloakOptions, logger)
    {
    }

    public async Task Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        await DeleteUserAsync(user, cancellationToken);
    }
}
