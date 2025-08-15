
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

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
    public ApiFactory()
    {
        _client = CreateClient();
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
    }

    public async Task InitializeAsync()
    {
        await database.StartAsync();
    }
}

[CollectionDefinition("ApiCollectionTests")]
public class ApiCollection
    : ICollectionFixture<ApiFactory>
{

}