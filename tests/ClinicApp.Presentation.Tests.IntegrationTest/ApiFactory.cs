
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure.Extensions;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Testcontainers.PostgreSql;

namespace ClinicApp.Presentation.Tests.IntegrationTest;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public const string ApiCollectionTests = "ApiCollectionTests";
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithImage("postgres:16.1-alpine3.19")
        .WithUsername("TestUser")
        .WithPassword("TestMe")
        .WithDatabase("ClinicDb")
        .Build();
    private HttpClient _client = null!;
    public HttpClient Client => _client;

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_database.GetConnectionString())
            .Options;
        // This assumes AppDbContext has a constructor that accepts DbContextOptions<AppDbContext>
        return new AppDbContext(options);
    }

    private void AttachJwtTokenToClient()
    {
        string jwt = Environment.GetEnvironmentVariable("Test_ClinicApp_Token")!;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", Serilog.Events.LogEventLevel.Debug)
            .WriteTo.Console()
            .CreateLogger();
        builder.UseSerilog();
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var exceptionHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType.FullName == "Microsoft.AspNetCore.Diagnostics.IExceptionHandler");
            if (exceptionHandlerDescriptor != null)
            {
                services.Remove(exceptionHandlerDescriptor);
            }

            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(_database.GetConnectionString());
            });

            services.RemoveAll<IClock>();
            services.AddSingleton<IClock, TestClock>();

            //remove caching using redis and using memory instead
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
        });
        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        
        // Seed the database BEFORE the client is created. This prevents the app
        // from holding active DB connections which would block schema changes.
        await SeedAsync();

        _client = CreateClient();
        AttachJwtTokenToClient();
    }

    public new async Task DisposeAsync()
    {
        // Ensure the database is cleaned up before we stop the container.
        await DeleteDatabaseAsync();
        await _database.DisposeAsync();
    }

    private async Task DeleteDatabaseAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.EnsureDeletedAsync();
    }

    private async Task SeedAsync()
    {
        await using var context = CreateDbContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync(); 
        InfrastructureMiddlewareExtensions.SeedDataToDbContext(context, TestClock.Clock());
    }
}

[CollectionDefinition("ApiCollectionTests")]
public class ApiCollection
    : ICollectionFixture<ApiFactory>
{

}

public class TestClock : IClock
{
    public DateTimeOffset UtcNow => new DateTimeOffset(2024,11,5,9,30,00,TimeSpan.FromHours(0));

    public static TestClock Clock()
    {
        return new TestClock();
    }
}