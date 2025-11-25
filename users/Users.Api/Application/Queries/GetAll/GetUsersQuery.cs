using MediatR;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetAll;

public class GetUsersRequest : IRequest<List<User>>;