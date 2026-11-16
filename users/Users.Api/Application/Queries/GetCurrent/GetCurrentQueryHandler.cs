using MediatR;
using Users.Api.Application.Queries.Dto;
using Users.Api.Extensions;
using Users.Api.Mapping;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetCurrent;

public sealed class GetCurrentQueryHandler(
    IHttpContextAccessor contextAccessor,
    IUsersRepository usersRepository) : IRequestHandler<GetCurrentQuery, UserReadDto>
{
    public async Task<UserReadDto> Handle(GetCurrentQuery request, CancellationToken cancellationToken)
    {
        var authProviderId = contextAccessor.GetAuthProviderId();

        var user = await usersRepository.GetByAuthProviderIdAsync(authProviderId, cancellationToken)
            ?? throw new NullReferenceException($"User with auth provider id {authProviderId} not found.");

        return user.ToReadDto();
    }
}