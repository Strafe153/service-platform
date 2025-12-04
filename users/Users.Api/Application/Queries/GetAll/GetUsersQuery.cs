using MediatR;
using Users.Api.Application.Queries.Dto;

namespace Users.Api.Application.Queries.GetAll;

public record GetUsersRequest(
    int PageNumber,
    int PageSize) : IRequest<PageDto<UserReadDto>>;