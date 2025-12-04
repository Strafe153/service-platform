using MassTransit.Initializers;
using MediatR;
using Users.Api.Application.Queries.Dto;
using Users.Api.Mapping;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetAll;

public class GetUsersQueryHandler : IRequestHandler<GetUsersRequest, PageDto<UserReadDto>>
{
    private readonly IUsersRepository _usersRepository;
    
    public GetUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public Task<PageDto<UserReadDto>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var query = _usersRepository.Get();

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