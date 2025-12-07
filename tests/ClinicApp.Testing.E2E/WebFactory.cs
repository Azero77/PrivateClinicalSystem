using API.Models;
using ClinicApp.Presentation.Tests.IntegrationTest;
using ClinicApp.Resources.Testing.Integration;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClinicApp.Testing.E2E;

public class WebFactory : IAsyncLifetime
{
    private ApiFactory _sessionApiFactory;
    private ResourcesApiFactory _resourcesApiFactory;
    public S3Settings GetS3Settings() => 
        _resourcesApiFactory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<IOptions<S3Settings>>().Value;

    public ApiFactory SessionApiFactory => _sessionApiFactory;
    public ResourcesApiFactory ResourcesApiFactory => _resourcesApiFactory;
    public WebFactory()
    {
        _sessionApiFactory = new ApiFactory();
        _resourcesApiFactory = new ResourcesApiFactory();
    }

    public async Task DisposeAsync()
    {
        await _sessionApiFactory.DisposeAsync();
        await _resourcesApiFactory.DisposeAsync();
    }
    public async Task InitializeAsync()
    {
        await _resourcesApiFactory.InitializeAsync();
        await _sessionApiFactory.InitializeAsync();
    }
}

[CollectionDefinition(nameof(WebFactoryCollectionDefinition))]
public class WebFactoryCollectionDefinition : ICollectionFixture<WebFactory>
{
}