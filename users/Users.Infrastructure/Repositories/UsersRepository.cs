using Microsoft.EntityFrameworkCore;
using Users.Application.Exceptions;
using Users.Domain.Aggregates.User;

namespace Users.Infrastructure.Repositories;

public class UsersRepository(DatabaseContext context) : IUsersRepository
{
    public IQueryable<User> Get() => context.Users;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByAuthProviderIdAsync(string id, CancellationToken cancellationToken) =>
        context.Users.FirstOrDefaultAsync(u => u.AuthProviderId == id, cancellationToken);

    public void Add(User user) => context.Users.Add(user);

    public void Update(User user) => context.Entry(user).State = EntityState.Modified;

    public void Delete(User user) => context.Users.Remove(user);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new DatabaseException(ex);
        }
    }
}