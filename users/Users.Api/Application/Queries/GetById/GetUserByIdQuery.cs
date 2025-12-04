using MediatR;
using Users.Api.Application.Queries.Dto;

namespace Users.Api.Application.Queries.GetById;

public sealed record GetUserByIdQuery(Ulid Id) : IRequest<UserReadDto>;