using MediatR;
using Users.Application.Queries.Dto;

namespace Users.Application.Queries.GetById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserReadDto>;