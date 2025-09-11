
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure.Extensions;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ClinicApp.Presentation.Tests.IntegrationTest;


public class ApiFactory : WebApplicationFactory<IApiMarker>,IAsyncLifetime

{
    public const string ApiCollectionTests = "ApiCollectionTests";
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder()
        .WithImage("postgres:16.1-alpine3.19")
        .WithUsername("TestUser")
        .WithPassword("TestMe")
        .WithDatabase("ClinicDb")
        .Build();
    private HttpClient _client = null!;
    public HttpClient Client => _client;

    private AppDbContext GetDbContext()
    {
        var scope = this.Services.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return dbcontext;
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
            //.WriteTo.TestOutput(_outputHelper)
            .CreateLogger();
        builder.UseSerilog();
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureTestServices(services =>
        {
            // Remove the default exception handler to see detailed errors in tests
            var exceptionHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType.FullName == "Microsoft.AspNetCore.Diagnostics.IExceptionHandler");
            if (exceptionHandlerDescriptor != null)
            {
                services.Remove(exceptionHandlerDescriptor);
            }

            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(database.GetConnectionString(),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            services.RemoveAll<IClock>();
            services.AddSingleton<IClock, TestClock>();
        });
        base.ConfigureWebHost(builder);
    }
    public new async Task DisposeAsync()
    {
        await database.DisposeAsync();

        //When tests are finished database is deleted
        await DeleteDatabase();
    }

    private async Task DeleteDatabase()
    {
        using var context = GetDbContext();
        await context.Database.EnsureDeletedAsync();
    }

    public async Task InitializeAsync()
    {
        
        await database.StartAsync();
        _client = CreateClient();
        AttachJwtTokenToClient();
        //Seed Data
        await Seed();
    }

    private async Task Seed()
    {
        using var context = GetDbContext();
        if (context.Database.GetPendingMigrations().Any())
            await context.Database.MigrateAsync();
        //InfrastructureMiddlewareExtensions.SeedDataToDbContext(context,TestClock.Clock());
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