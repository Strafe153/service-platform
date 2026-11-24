using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Users.Application.Abstractions;
using Users.Application.Constants;
using Users.Application.Exceptions;
using Users.Application.Mapping;
using Users.Application.Models;
using Users.Application.Queries.Dto;
using Users.Application.Telemetry;
using Users.Domain.Aggregates.User;
using Users.Domain.Events;

namespace Users.Application.Commands.Create;

public sealed class CreateUserCommandHandler(
    IUsersRepository usersRepository,
    IMessagePublisher messagePublisher,
    IUserService userService,
    ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, UserReadDto>
{
    public async Task<UserReadDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        using var activity = UsersTelemetry.ActivitySource.StartActivity("users.create");

        var userId = string.Empty;

        try
        {
            UserRequest request = new()
            {
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
            };

            userId = await userService.CreateUserAsync(request, cancellationToken);

            await userService.ResetPasswordAsync(userId, command.Password, cancellationToken);
            var dbUser = await SaveUserAsync(command, userId, cancellationToken);

            await PublishUserCreatedEventAsync(command, cancellationToken);

            logger.LogInformation("Successfully created a user");
            UsersTelemetry.UsersCreated.Add(1);

            activity?
                .SetTag("user.keycloak_id", userId)
                .SetTag("user.id", dbUser.Id)
                .SetStatus(ActivityStatusCode.Ok);

            return dbUser.ToReadDto();
        }
        catch (DatabaseException)
        {
            logger.LogError("Failed to create a user due to a database failure");
            UsersTelemetry.UserCreationFailures.IncrementError();
            activity?.SetStatus(ActivityStatusCode.Error, "Database update failed");

            await userService.DeleteUserAsync(userId, cancellationToken);

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

    private Task PublishUserCreatedEventAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        UserCreatedEvent userCreatedEvent = new(command.Email, DateTime.UtcNow);

        var eventTask = messagePublisher.Publish(
            userCreatedEvent,
            MessagingKeys.UserCreated,
            cancellationToken);

        return eventTask;
    }
}
