using System.Net;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Keycloak;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Users.Infrastructure;
using Xunit;

namespace Integration.Tests;

public class UsersWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const int InternalKeycloakPort = 8080;
    private const string RabbitMqUser = "mquser";
    private const string RabbitMqPassword = "mqpass";

    private readonly MsSqlContainer _msSqlContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    private readonly RabbitMqContainer _rabbitMqcontainer = new RabbitMqBuilder("rabbitmq:4.2.0-management")
        .WithUsername(RabbitMqUser)
        .WithPassword(RabbitMqPassword)
        .Build();

    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder("quay.io/keycloak/keycloak:26.4")
        .WithEnvironment("KEYCLOAK_ADMIN", "admin")
        .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "qwerty")
        .WithCommand()
        .WithBindMount(RealmFilePath, "/opt/keycloak/data/import/users_realm.json")
        .WithCommand("--import-realm")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
            request
                .ForPort(InternalKeycloakPort)
                .ForPath("/realms/users/.well-known/openid-configuration")
                .ForStatusCode(HttpStatusCode.OK)))
        .Build();

    public static Guid AdminId => Guid.Parse("01b940e9-4cb8-4035-97d8-59ceb16a4338");
    public static Guid TestId => Guid.Parse("7e5054cb-1609-4979-a40d-c0db5cffa938");
    public static Guid JokerId => Guid.Parse("d2399895-cdfb-4329-a487-9d94ab31fbe2");
    public static Guid CrowId => Guid.Parse("2e26ecc2-3476-49e1-bab5-4f97d031a2c3");
    public static Guid FoxId => Guid.Parse("df07d570-eb42-457e-89fd-51781ab7db94");

    public string KeycloakTokenUrl
    {
        get
        {
            var port = _keycloakContainer.GetMappedPublicPort(InternalKeycloakPort);
            return $"http://localhost:{port}/realms/users/protocol/openid-connect/token";
        }
    }

    private static string RealmFilePath
    {
        get
        {
            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "users_realm.json");

            return filePath;
        }
    }

    public async ValueTask InitializeAsync()
    {
        await Task.WhenAll(
            _msSqlContainer.StartAsync(),
            _rabbitMqcontainer.StartAsync(),
            _keycloakContainer.StartAsync());

        await SeedDataAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await Task.WhenAll(
            _msSqlContainer.StopAsync(),
            _rabbitMqcontainer.StopAsync(),
            _keycloakContainer.StopAsync());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(options =>
        {
            var connectionString = _msSqlContainer.GetConnectionString();

            options
                .RemoveAll(typeof(DbContextOptions<DatabaseContext>))
                .AddDbContext<DatabaseContext>(b => b.UseSqlServer(connectionString));
        });

        builder.ConfigureAppConfiguration((ctx, b) =>
        {
            var keycloakPort = _keycloakContainer.GetMappedPublicPort(InternalKeycloakPort);
            var keycloakAddress = $"http://localhost:{keycloakPort}/";

            var rabbitMqPort = _rabbitMqcontainer.GetMappedPublicPort().ToString();

            Dictionary<string, string?> configs = new()
            {
                ["Keycloak:auth-server-url"] = keycloakAddress,
                ["Keycloak:credentials:secret"] = "vGFERwcZYrCJdnWebH9JB9EaSr8AQf8C",

                ["KeycloakAdmin:auth-server-url"] = keycloakAddress,
                ["KeycloakAdmin:realm"] = "users",
                ["KeycloakAdmin:credentials:secret"] = "R1SzmswuIeEX5sKFvcvbnUUzQUlAKIdX",

                ["RabbitMq:Port"] = rabbitMqPort,
                ["RabbitMq:Username"] = RabbitMqUser,
                ["RabbitMq:Password"] = RabbitMqPassword
            };

            b.AddInMemoryCollection(configs);
        });
    }

    private async Task SeedDataAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await context.Database.MigrateAsync();

        await using var transaction = await context.Database.BeginTransactionAsync();

#pragma warning disable EF1002
        await context.Database.ExecuteSqlRawAsync($"""
            INSERT INTO users
            (
                Id,
                Email,
                FirstName,
                LastName,
                PhoneNumber,
                BirthDate,
                AuthProviderId
            )
            VALUES
            (
                '{AdminId}',
                'admin@mail.com',
                'Admin',
                'Admin',
                '0953189734',
                '1989-10-11',
                'f998a5c2-9173-4587-8724-c609750fe8b9'
            ),
            (
                '{TestId}',
                'test@mail.com',
                'Test',
                'User',
                '0987023981',
                '1997-08-24',
                'b3cb8ebf-9c74-416a-a888-b128cdb218e5'
            ),
            (
                '{JokerId}',
                'joker@mail.com',
                'Ren',
                'Amamiya',
                '0987654321',
                '2000-02-19',
                '20fb8609-cc35-406e-ada6-92f3cef5abec'
            ),
            (
                '{CrowId}',
                'crow@pubsec.com',
                'Goro',
                'Akechi',
                '0668972341',
                '1999-05-07',
                '5d749445-df56-4ec1-b70a-2c332fc5b31d'
            ),
            (
                '{FoxId}',
                'fox@phantoms.com',
                'Yusuke',
                'Kitagawa',
                '0991098277',
                '2000-09-29',
                '2e34290a-4c44-4f8c-add4-7839f36559ee'
            )

            INSERT INTO Addresses
            (
                UserId,
                Country,
                State,
                City,
                ZipCode
            )
            VALUES
            (
                '{AdminId}',
                'Ukraine',
                'Kyiv Oblast',
                'Kyiv',
                '01001'
            ),
            (
                '{TestId}',
                'USA',
                'California',
                'Torrance',
                '90501'
            ),
            (
                '{JokerId}',
                'Japan',
                'Tokyo',
                'Yongenjaya',
                '100-1234'
            ),
            (
                '{CrowId}',
                'Japan',
                'Tokyo',
                'Shinjuku',
                '101-8656'
            ),
            (
                '{FoxId}',
                'Japan',
                'Tokyo',
                'Jinbocho',
                '101-0051'
            )
        """);
#pragma warning restore EF1002

        await transaction.CommitAsync();
        await context.SaveChangesAsync();
    }
}