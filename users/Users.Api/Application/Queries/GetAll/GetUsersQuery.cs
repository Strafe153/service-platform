using MediatR;
using Users.Api.Application.Queries.Dto;

namespace Users.Api.Application.Queries.GetAll;

public record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PageDto<UserReadDto>>;