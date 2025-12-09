using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using API;
using API.Models;
using DotNet.Testcontainers.Builders;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.LocalStack;

namespace ClinicApp.Resources.Testing.Integration;
public class ResourcesApiFactory : WebApplicationFactory<IResourceApiMarker>, IAsyncLifetime

{
    //starting localstack container
    LocalStackContainer LocalStackContainer = new LocalStackBuilder()
            .WithImage("localstack/localstack")
            .Build();
    public ResourcesApiFactory()
    {
    }

    public async Task InitializeAsync()
    {
        //configure s3 bucket
        await LocalStackContainer.StartAsync();
        var s3Client = this.Services.CreateScope()
            .ServiceProvider.GetRequiredService<IAmazonS3>();

        var s3Settings = Services.CreateScope()
            .ServiceProvider.GetRequiredService<IOptions<S3Settings>>().Value;


        await s3Client.PutBucketAsync(new PutBucketRequest()
        {
            BucketName = s3Settings.BucketName,
        });
        
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        
        builder.ConfigureServices(services =>
        {
            services.Configure<S3Settings>(s3 =>
            {
                s3.BucketName = "session-content-test-bucket";
                s3.ForcePathStyle = true;
                s3.Region = "us-east-1";
                s3.ServiceUrl = LocalStackContainer.GetConnectionString();
            });
            services.RemoveAll<IAmazonS3>();
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
                var credentials = new Amazon.Runtime.BasicAWSCredentials("test", "test");
                return new AmazonS3Client(credentials,new AmazonS3Config()
                {
                    ServiceURL = s3Settings.ServiceUrl,
                    ForcePathStyle = s3Settings.ForcePathStyle,
                });
            });

        });
        base.ConfigureWebHost(builder);
    }

    public new Task DisposeAsync()
    {
        return LocalStackContainer.DisposeAsync().AsTask();
    }
}
