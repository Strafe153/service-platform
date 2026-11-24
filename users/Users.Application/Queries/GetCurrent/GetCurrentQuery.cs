using MediatR;
using Users.Application.Queries.Dto;

public sealed record GetCurrentQuery : IRequest<UserReadDto>
{
    public static GetCurrentQuery Instance { get; } = new();
}