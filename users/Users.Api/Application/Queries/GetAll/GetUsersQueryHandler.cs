using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Queries.GetAll;

public class GetUsersQueryHandler : IRequestHandler<GetUsersRequest, List<User>>
{
    private readonly IUsersRepository _usersRepository;
    
    public GetUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public Task<List<User>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var users = _usersRepository.Get().ToListAsync(cancellationToken: cancellationToken);
        return users;
    }
}