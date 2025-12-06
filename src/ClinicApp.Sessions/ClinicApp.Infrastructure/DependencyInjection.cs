using Amazon.SimpleNotificationService;
using Amazon.SQS;
using ClinicApp.Application.Common;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.QueryServices;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SecretaryAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Common;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Infrastructure.QueryServices;
using ClinicApp.Infrastructure.Repositories;
using ClinicApp.Infrastructure.Services;
using ClinicApp.Infrastructure.SettingsConfiguration;
using ClinicApp.Shared;
using ClinicApp.Shared.QueryTypes;
using MassTransit;
using MassTransit.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;

namespace ClinicApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<AppDbContext>((sp, opts) =>
        {
            opts.UseNpgsql(builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("Connection string is null for clinic db"));
        });

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = builder.Configuration.GetConnectionString("cache") ?? throw new ArgumentException("Connection string is null for redis ");
            opts.InstanceName = "clinic-sessions";
        });

        services.AddScoped<ISessionRepository, DbSessionRepository>();
        services.AddScoped<DbDoctorRepository>();
        services.AddScoped<IDoctorRepository, CachedDbDoctorRepository>();
        services.AddScoped<DbRoomRepository>();
        services.AddScoped<IRoomRepository, CachedDbRoomRepository>();
        services.AddScoped<IPatientRepository, DbPatientRepository>();
        services.AddScoped<ISecretaryRepository, DbSecretaryRepository>();
        services.AddSingleton<IClock, Clock>();


        services.AddSingleton<IConverter<Doctor, DoctorDataModel>, DoctorConverter>();
        services.AddSingleton<IConverter<Room, RoomDataModel>, RoomConverter>();
        services.AddSingleton<IConverter<Session, SessionDataModel>, SessionConverter>();
        services.AddSingleton<IConverter<Patient, PatientDataModel>, PatientConverter>();
        services.AddSingleton<IConverter<Secretary, SecretaryDataModel>, SecretaryConverter>();
        services.AddScoped<IQueryService<SessionQueryType>, SessionQueryService>();
        services.AddScoped<IQueryService<DoctorQueryType>, DoctorQueryService>();
        services.AddScoped<IQueryService<RoomQueryType>, RoomQueryService>();
        services.AddScoped<IQueryService<SecretaryQueryType>, SecretaryQueryService>();
        services.AddScoped<IQueryService<PatientQueryType>, PatientQueryService>();
        services.AddScoped<IEventAdderService<SessionDomainEvent>, SessionEventAdderService>();
        //Mediatr is unable to register generic requestHandlers because DI with MSDI can support this kind of stuff
        AddHttpClients(services, builder);
        services.AddMessaging(builder);
        return services;
    }

    private static void AddHttpClients(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<ClientConfiguration>(builder.Configuration.GetSection(ClientConfiguration.ClientConfigurationSectionName));
        RegisterMediatrGenericHandlers(services);
        services.AddHttpClient<IResourcesClientService, HttpResourcesClientService>(HttpResourcesClientService.HttpResourceClientServiceClientName, (sp, client) =>
        {
            client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ClientConfiguration>>()
            .Value.ResourceClientBaseUrl);
        })
            .AddStandardResilienceHandler();
    }

    private static void RegisterMediatrGenericHandler<T>(IServiceCollection services)
        where T:QueryType
    {
        services.AddScoped<IRequestHandler<QueryRequest<T>, IQueryable<T>>, QueryRequestHandler<T>>();
        services.AddScoped<IRequestHandler<QuerySingleRequest<T>, T?>, QuerySingleRequestHandler<T>>();

    }
    private static void RegisterMediatrGenericHandlers(IServiceCollection services)
    {
        RegisterMediatrGenericHandler<SessionQueryType>(services);
        RegisterMediatrGenericHandler<DoctorQueryType>(services);
        RegisterMediatrGenericHandler<PatientQueryType>(services);
        RegisterMediatrGenericHandler<RoomQueryType>(services);
        RegisterMediatrGenericHandler<SecretaryQueryType>(services);
    }
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
            opts.UsingAmazonSqs((context,config) =>
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
                config.ConfigureEndpoints(context);
            });


            opts.AddConsumers(typeof(Application.DependencyInjection).Assembly);
        });

        return services;

        
    }
}