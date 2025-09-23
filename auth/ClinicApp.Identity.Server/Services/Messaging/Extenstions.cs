using MassTransit;

namespace ClinicApp.Identity.Server.Services.Messaging;

public static class Extenstions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(opts =>
        {
            opts.UsingRabbitMq((context,cfg) =>
            {
                var configuration = context.GetService<IConfiguration>();
                if (configuration is null)
                    throw new ArgumentException();
                var connectionString = configuration.GetConnectionString("rabbitmq");
                cfg.Host(connectionString);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
