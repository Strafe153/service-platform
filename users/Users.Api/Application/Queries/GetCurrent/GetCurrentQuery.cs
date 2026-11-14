using MediatR;
using Users.Api.Application.Queries.Dto;

public sealed record GetCurrentQuery : IRequest<UserReadDto>
{
    public static GetCurrentQuery Instance { get; } = new();
}