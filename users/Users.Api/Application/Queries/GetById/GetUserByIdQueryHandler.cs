using MediatR;
using Users.Api.Application.Queries.Dto;
using Users.Api.Mapping;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserReadDto>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserByIdQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<UserReadDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        return user.ToReadDto();
    }
}