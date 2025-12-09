using MassTransit;
using RabbitMQ.Client;
using Users.Domain.Events;

namespace Users.Api.Configurations.Messaging;

public static class MassTransitConfiguration
{
    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.UsingRabbitMq((context, cfg) =>
            {
                var options = configuration.GetSection(ConfigConstants.RabbitMq).Get<RabbitMqOptions>()!;

                cfg.Host(options.Host, c =>
                {
                   c.Username(options.Username);
                   c.Password(options.Password); 
                });

                cfg.Message<UserCreatedEvent>(c => c.SetEntityName(RabbitMqConstants.Exchanges.User));
                cfg.Publish<UserCreatedEvent>(c => c.ExchangeType = ExchangeType.Direct);

                cfg.Message<UserDeletedEvent>(c => c.SetEntityName(RabbitMqConstants.Exchanges.User));
                cfg.Publish<UserDeletedEvent>(c => c.ExchangeType = ExchangeType.Direct);

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}