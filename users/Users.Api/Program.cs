using FluentValidation;
using Users.Api;
using Users.Api.Configurations;
using Users.Api.Configurations.Authorization;
using Users.Api.Configurations.Messaging;
using Users.Api.Endpoints;
using Users.Domain.Aggregates.User;
using Users.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureKeycloak(builder.Configuration);
builder.Services.ConfigureMassTransit(builder.Configuration);
builder.Services.ConfigureMediatR();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<ExceptionHandler>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(_ => {});

app.RegisterUserEndpoints();

await app.RunDatabaseMigrations();

app.Run();