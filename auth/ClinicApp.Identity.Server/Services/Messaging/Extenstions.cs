using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleEmail;
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
            .GetSection("AWS")
            .Get<AwsConfiguration>() ?? throw new ArgumentException();
            opts.UsingAmazonSqs((context, config) =>
            {

                config.Host(awsConfiguration.Region, h =>
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
        });

        return services;
    }

    public static IServiceCollection AddEmailing(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.EmailConfigurationSection));
        services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSimpleEmailService>();


        return services;
    }
}


public class EmailSettings
{
    public const string EmailConfigurationSection = "EmailSettings";
    public string SenderEmail { get; set; } = string.Empty;
}