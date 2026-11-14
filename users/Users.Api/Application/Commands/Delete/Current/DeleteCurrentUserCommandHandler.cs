using System.Security.Claims;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.Delete.Current;

public sealed class DeleteCurrentUserCommandHandler
    : DeleteUserCommandHandler, IRequestHandler<DeleteCurrentUserCommand>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public DeleteCurrentUserCommandHandler(
        IHttpContextAccessor contextAccessor,
        IUsersRepository usersRepository,
        IKeycloakClient keycloakClient,
        IPublishEndpoint publishEndpoint,
        IOptions<KeycloakAuthenticationOptions> keycloakOptions,
        ILogger<DeleteCurrentUserCommandHandler> logger)
            : base(usersRepository, keycloakClient, publishEndpoint, keycloakOptions, logger)
    {
        _contextAccessor = contextAccessor;
    }

    public async Task Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.delete");

        var authProviderId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (authProviderId is null || string.IsNullOrEmpty(authProviderId.Value))
        {
            throw new NullReferenceException("No auth provider id passed.");
        }

        var user = await _usersRepository.GetByAuthProviderIdAsync(authProviderId.Value, cancellationToken)
            ?? throw new NullReferenceException($"User with id auth provider id {authProviderId.Value} not found.");

        await DeleteUserAsync(user, cancellationToken);
    }
}