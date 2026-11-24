using MediatR;
using Users.Application.Queries.Dto;

namespace Users.Application.Queries.GetAll;

public record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PageDto<UserReadDto>>;