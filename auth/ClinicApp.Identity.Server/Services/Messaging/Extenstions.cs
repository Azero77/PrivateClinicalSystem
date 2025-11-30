using Amazon.SimpleNotificationService;
using Amazon.SQS;
using ClinicApp.Shared;
using MassTransit;

namespace ClinicApp.Identity.Server.Services.Messaging;

public static class Extenstions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddMassTransit(opts =>
        {
            /*opts.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetService<IConfiguration>();
                if (configuration is null)
                    throw new ArgumentException();
                var connectionString = configuration.GetConnectionString("rabbitmq");
                cfg.Host(connectionString);
                cfg.ConfigureEndpoints(context);
               
            });*/


            AwsConfiguration awsConfiguration = builder.Configuration
            .GetSection("AwsConfiguration")
            .Get<AwsConfiguration>() ?? throw new ArgumentException();
            opts.UsingAmazonSqs((context, config) =>
            {

                config.Host(awsConfiguration.DefaultOrigin, h =>
                {

                    h.Config(new AmazonSQSConfig
                    {
                        ServiceURL = awsConfiguration.ServiceUrl
                    });

                    h.Config(new AmazonSimpleNotificationServiceConfig
                    {
                        ServiceURL = awsConfiguration.ServiceUrl
                    });
                    h.SecretKey(awsConfiguration.SecretKey);
                    h.AccessKey(awsConfiguration.AccessKey);
                });
            });


            opts.AddConsumers(typeof(Application.DependencyInjection).Assembly);
        });

        return services;
    }
}
