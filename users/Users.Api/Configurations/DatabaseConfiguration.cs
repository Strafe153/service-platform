using Microsoft.EntityFrameworkCore;
using Users.Infrastructure;

namespace Users.Api.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfigSection = configuration.GetConnectionString(ConfigConstants.Database);
        services.AddDbContext<DatabaseContext>(o => o.UseSqlServer(databaseConfigSection));
    }

    public static async Task RunDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}