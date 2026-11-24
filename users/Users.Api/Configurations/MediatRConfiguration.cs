using MediatR;
using Users.Application;
using Users.Application.Behaviors;

namespace Users.Api.Configurations;

public static class MediatRConfiguration
{
    public static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
    }
}