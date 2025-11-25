using MediatR;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetById;

public sealed record GetUserByIdQuery(int Id) : IRequest<User>;