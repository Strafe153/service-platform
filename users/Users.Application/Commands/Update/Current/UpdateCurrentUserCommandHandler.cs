using System.Diagnostics;
using MediatR;
using Users.Domain.Aggregates.User;
using Users.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Users.Application.Exceptions;
using Users.Application.Telemetry;

namespace Users.Application.Commands.Update.Current;

public sealed class UpdateCurrentUserCommandHandler(
    ICurrentUserService currentuserService,
    IUsersRepository usersRepository,
    ILogger<UpdateCurrentUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.update");

        var authProviderId = currentuserService.GetId();

        try
        {
            var user = await usersRepository.GetByAuthProviderIdAsync(authProviderId, cancellationToken)
                ?? throw new NullReferenceException($"User with auth provider id {authProviderId} not found.");

            user.Update(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.BirthDate);

            usersRepository.Update(user);
            await usersRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully updated the user with Id={Id}", user.Id);
            UsersTelemetry.UsersUpdated.Add(1);

            activity?
                .SetTag("user.id", user.Id)
                .SetTag("user.keycloak_id", user.AuthProviderId)
                .SetStatus(ActivityStatusCode.Ok);

            return Unit.Value;
        }
        catch (DatabaseException)
        {
            logger.LogError(
                "Failed to update the user with AuthProviderId={Id} due to a database failure",
                authProviderId);

            UsersTelemetry.UserUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Failed to update the user with AuthProviderId={Id} due to the exception: {Exception}",
                authProviderId,
                ex.Message);

            UsersTelemetry.UserUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }
}