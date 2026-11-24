using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Configurations.Authorization;
using Users.Application.Commands;
using Users.Application.Commands.Create;
using Users.Application.Commands.Delete.ById;
using Users.Application.Commands.Delete.Current;
using Users.Application.Commands.Update;
using Users.Application.Commands.UpdateAddress;
using Users.Application.Queries.Dto;
using Users.Application.Queries.GetAll;
using Users.Application.Queries.GetById;

namespace Users.Api.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        const string idSpecifier = "{id:guid}";

        var group = app.MapGroup("users");

        group.MapGet(string.Empty, Get)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        group.MapGet(idSpecifier, GetById)
            .WithName(nameof(GetById))
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        group.MapPost(string.Empty, Create);

        group.MapPut(idSpecifier, Update)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        group.MapPut($"{idSpecifier}/address", UpdateAddress)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        group.MapDelete(idSpecifier, Delete)
            .RequireAuthorization(KeycloakConstants.Policies.AdminOnly);

        var meGroup = group
            .MapGroup("me")
            .RequireAuthorization();

        meGroup.MapGet(string.Empty, GetCurrent);
        meGroup.MapPut(string.Empty, UpdateCurrent);
        meGroup.MapDelete(string.Empty, DeleteCurrent);
    }

    public static async Task<Ok<PageDto<UserReadDto>>> Get(
        [FromServices] ISender sender,
        [AsParameters] GetUsersQuery request,
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
        [FromRoute] Guid id,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        IdentifiedCommand<Guid, UpdateUserCommand, Unit> identifiedCommand = new(id, command);
        await sender.Send(identifiedCommand, cancellationToken);

        return TypedResults.NoContent();
    }

    public static async Task<NoContent> UpdateAddress(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateAddressCommand command,
        CancellationToken cancellationToken)
    {
        IdentifiedCommand<Guid, UpdateAddressCommand, Unit> identifiedCommand = new(id, command);
        await sender.Send(identifiedCommand, cancellationToken);

        return TypedResults.NoContent();
    }

    public static async Task<NoContent> Delete(
        [FromServices] ISender sender,
        [AsParameters] DeleteUserByIdCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<Ok<UserReadDto>> GetCurrent(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(GetCurrentQuery.Instance, cancellationToken);
        return TypedResults.Ok(user);
    }

    public static async Task<NoContent> UpdateCurrent(
        [FromServices] ISender sender,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteCurrent(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(DeleteCurrentUserCommand.Instance, cancellationToken);
        return TypedResults.NoContent();
    }
}