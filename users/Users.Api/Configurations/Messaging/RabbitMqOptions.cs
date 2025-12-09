namespace Users.Api.Configurations.Messaging;

public sealed class RabbitMqOptions
{
    public string Host { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;
}