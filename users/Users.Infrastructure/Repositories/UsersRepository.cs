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

    public Task<User?> GetByIdAsync(Ulid id, CancellationToken cancellationToken) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public void Add(User user) => _context.Users.Add(user);

    public void Update(User user) => _context.Entry(user).State = EntityState.Modified;

    public void Delete(User user) => _context.Users.Remove(user);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}