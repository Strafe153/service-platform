using MediatR;
using Users.Api.Application.Behaviors;

namespace Users.Api.Configurations;

public static class MediatRConfiguration
{
    public static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
    }
}