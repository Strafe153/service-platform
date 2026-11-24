using MediatR;
using Users.Application.Queries.Dto;
using Users.Application.Mapping;
using Users.Domain.Aggregates.User;

namespace Users.Application.Queries.GetById;

public sealed class GetUserByIdQueryHandler(IUsersRepository usersRepository)
    : IRequestHandler<GetUserByIdQuery, UserReadDto>
{
    public async Task<UserReadDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        return user.ToReadDto();
    }
}