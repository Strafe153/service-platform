using MediatR;
using Users.Api.Configurations;
using Users.Api.Application.Commands.Create;
using Users.Api.Application.Queries.GetAll;
using Users.Domain.Aggregates.User;
using Users.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureKeycloak(builder.Configuration);
builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("users", (ISender sender, CancellationToken token) =>
{
    var users = sender.Send(new GetUsersRequest(), token);
    return TypedResults.Ok(users);
}).RequireAuthorization("admin-only");

app.MapPost("users", async (ISender sender, CreateUserCommand command, CancellationToken token) =>
{
    var user = await sender.Send(command, token);
    // get by id doesn't exist yet
    return TypedResults.CreatedAtRoute(user, "Get", new { id = user.Id });
});

await app.RunDatabaseMigrations();

app.Run();