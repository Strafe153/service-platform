using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.Update;

public class UpdateUserCommandHandler(
    IUsersRepository usersRepository,
    ILogger<UpdateUserCommandHandler> logger)
        : IRequestHandler<IdentifiedCommand<Guid, UpdateUserCommand, Unit>, Unit>
{
    public async Task<Unit> Handle(
        IdentifiedCommand<Guid, UpdateUserCommand, Unit> request,
        CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.update");

        try
        {
            var user = await usersRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NullReferenceException($"User with id {request.Id} not found.");

            user.Update(
                request.Command.FirstName,
                request.Command.LastName,
                request.Command.PhoneNumber,
                request.Command.BirthDate);

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
        catch (DbUpdateException)
        {
            logger.LogError("Failed to update the user with Id={Id} due to a database failure", request.Id);
            UsersTelemetry.UserUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Failed to update the user with Id={Id} due to the exception: {Exception}",
                request.Id,
                ex.Message);

            UsersTelemetry.UserUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }
}
