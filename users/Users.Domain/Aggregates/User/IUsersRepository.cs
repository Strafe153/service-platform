namespace Users.Domain.Aggregates.User;

public interface IUsersRepository
{
    IQueryable<User> Get();

    Task<User?> GetByIdAsync(Ulid id, CancellationToken cancellationToken);

    Task<User?> GetByAuthProviderIdAsync(string id, CancellationToken cancellationToken);

    void Add(User user);

    void Update(User user);

    void Delete(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}