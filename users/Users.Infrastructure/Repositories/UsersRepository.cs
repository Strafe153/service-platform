using Microsoft.EntityFrameworkCore;
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

    public Task<User?> GetByIdAsync(Ulid id) => _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByAuthProviderIdAsync(string id) =>
        _context.Users.FirstOrDefaultAsync(u => u.AuthProviderId == id);

    public void Add(User user) => _context.Users.Add(user);

    public void Update(User user) => _context.Entry(user).State = EntityState.Modified;

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}