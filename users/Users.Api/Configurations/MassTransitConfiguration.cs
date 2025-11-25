using MassTransit;
using Users.Api.Configurations.Models;

namespace Users.Api.Configurations;

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

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}