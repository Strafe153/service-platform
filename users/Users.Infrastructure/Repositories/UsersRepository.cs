using Users.Domain.Aggregates.User;

namespace Users.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DatabaseContext _context;

    public UsersRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IQueryable<User> Get() => _context.Users;

    public void Add(User user) => _context.Users.Add(user);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}