namespace Users.Domain.Aggregates.User;

public interface IUsersRepository
{
    IQueryable<User> Get();

    Task<User?> GetByIdAsync(Ulid id);

    Task<User?> GetByAuthProviderIdAsync(string id);

    void Add(User user);

    void Update(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}