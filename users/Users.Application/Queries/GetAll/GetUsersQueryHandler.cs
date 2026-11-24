using MediatR;
using Users.Application.Queries.Dto;
using Users.Application.Mapping;
using Users.Domain.Aggregates.User;

namespace Users.Application.Queries.GetAll;

public sealed class GetUsersQueryHandler(IUsersRepository usersRepository)
    : IRequestHandler<GetUsersQuery, PageDto<UserReadDto>>
{
    public Task<PageDto<UserReadDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = usersRepository.Get();

        var dtos = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(UserExtensions.ToReadDto)
            .ToList();
            
        PageDto<UserReadDto> page = new(
            request.PageNumber,
            request.PageSize,
            dtos,
            query.Count());

        return Task.FromResult(page);
    }
}