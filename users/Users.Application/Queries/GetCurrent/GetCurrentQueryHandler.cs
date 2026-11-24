using MediatR;
using Users.Application.Queries.Dto;
using Users.Application.Mapping;
using Users.Domain.Aggregates.User;
using Users.Application.Abstractions;

namespace Users.Application.Queries.GetCurrent;

public sealed class GetCurrentQueryHandler(
    ICurrentUser currentUser,
    IUsersRepository usersRepository) : IRequestHandler<GetCurrentQuery, UserReadDto>
{
    public async Task<UserReadDto> Handle(GetCurrentQuery request, CancellationToken cancellationToken)
    {
        var authProviderId = currentUser.GetId();

        var user = await usersRepository.GetByAuthProviderIdAsync(authProviderId, cancellationToken)
            ?? throw new NullReferenceException($"User with auth provider id {authProviderId} not found.");

        return user.ToReadDto();
    }
}