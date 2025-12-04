using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Application.Commands;
using Users.Api.Application.Commands.Create;
using Users.Api.Application.Commands.Update;
using Users.Api.Application.Commands.UpdateAddress;
using Users.Api.Application.Queries.Dto;
using Users.Api.Application.Queries.GetAll;
using Users.Api.Application.Queries.GetById;
using Users.Api.Configurations.Authorization;

namespace Users.Api.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        const string idSpecifier = "{id:length(26)}";

        var group = app.MapGroup("users");

        group.MapGet(string.Empty, Get)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        group.MapGet(idSpecifier, GetById)
            .WithName(nameof(GetById))
            .RequireAuthorization(KeycloakConstants.Policies.AdminOrRequestedUser);

        group.MapPost(string.Empty, Create);

        group.MapPut(idSpecifier, Update)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOrRequestedUser);

        group.MapPut($"{idSpecifier}/address", UpdateAddress)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOrRequestedUser);
    }

    public static async Task<Ok<PageDto<UserReadDto>>> Get(
        [FromServices] ISender sender,
        [AsParameters] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var users = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(users);
    }

    public static async Task<Ok<UserReadDto>> GetById(
        [FromServices] ISender sender,
        [AsParameters] GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(user);
    }

    public static async Task<CreatedAtRoute<UserReadDto>> Create(
        [FromServices] ISender sender,
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(command, cancellationToken);
        return TypedResults.CreatedAtRoute(user, nameof(GetById), new { id = user.Id });
    }

    public static async Task<NoContent> Update(
        [FromServices] ISender sender,
        [FromRoute] Ulid id,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        IdentifiedCommand<Ulid, UpdateUserCommand> identifiedCommand = new(id, command);
        await sender.Send(identifiedCommand, cancellationToken);

        return TypedResults.NoContent();
    }

    public static async Task<NoContent> UpdateAddress(
        [FromServices] ISender sender,
        [FromRoute] Ulid id,
        [FromBody] UpdateAddressCommand command,
        CancellationToken cancellationToken)
    {
        IdentifiedCommand<Ulid, UpdateAddressCommand> identifiedCommand = new(id, command);
        await sender.Send(identifiedCommand, cancellationToken);

        return TypedResults.NoContent();
    }
}