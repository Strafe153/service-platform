using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.Api.Telemetry;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.UpdateAddress;

public class UpdateAddressCommandHandler(
    IUsersRepository usersRepository,
    ILogger<UpdateAddressCommandHandler> logger)
        : IRequestHandler<IdentifiedCommand<Guid, UpdateAddressCommand, Unit>, Unit>
{
    public async Task<Unit> Handle(
        IdentifiedCommand<Guid, UpdateAddressCommand, Unit> request,
        CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.address.update");

        try
        {
            var user = await usersRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NullReferenceException($"User with id {request.Id} not found.");

            user.UpdateAddress(
                request.Command.Country,
                request.Command.State,
                request.Command.City,
                request.Command.ZipCode,
                request.Command.Street);

            usersRepository.Update(user);
            await usersRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully updated the address of the user with Id={Id}", user.Id);
            UsersTelemetry.UserAddressUpdated.Add(1);

            activity?
                .SetTag("user.id", user.Id)
                .SetTag("user.keycloak_id", user.AuthProviderId)
                .SetStatus(ActivityStatusCode.Ok);

            return Unit.Value;
        }
        catch (DbUpdateException)
        {
            logger.LogError(
                "Failed to update the address of the user with Id={Id} due to a database failure",
                request.Id);

            UsersTelemetry.UserAddressUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Failed to update the address of user with Id={Id} due to the exception: {Exception}",
                request.Id,
                ex.Message);

            UsersTelemetry.UserAddressUpdateFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }
}
