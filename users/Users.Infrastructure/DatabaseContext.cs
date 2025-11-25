using Microsoft.EntityFrameworkCore;
using Users.Domain.Aggregates.User;
using Users.Infrastructure.Configurations;

namespace Users.Infrastructure;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; init; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersConfiguration).Assembly);
    }
}