namespace Users.Domain.Aggregates.User;

public interface IUsersRepository
{
    IQueryable<User> Get();

    void Add(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}