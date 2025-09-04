
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
    private readonly HttpClient _client;
    public HttpClient Client => _client;

    private AppDbContext GetDbContext()
    {
        var scope = this.Services.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return dbcontext;
    }
    public ApiFactory()
    {
        _client = CreateClient();
        AttachJwtTokenToClient();
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
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(database.GetConnectionString());
            });
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

        //Seed Data
        await Seed();
    }

    private async Task Seed()
    {
        using var context = GetDbContext();
        await context.Database.EnsureCreatedAsync();
        DbExtensions.SeedDataToDbContext(context);
    }
}

[CollectionDefinition("ApiCollectionTests")]
public class ApiCollection
    : ICollectionFixture<ApiFactory>
{

}