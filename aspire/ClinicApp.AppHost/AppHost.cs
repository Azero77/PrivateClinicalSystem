using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var postgresClinic = builder.AddPostgres("postgres")
    .WithImage("postgres:16.1-alpine3.19")
    .WithPgAdmin()
    .WithDataVolume(isReadOnly : false);

var pgadmin = postgresClinic.WithPgAdmin();
var db = postgresClinic.AddDatabase("postgresClinicdb");

var identityServer = builder.AddProject<Projects.ClinicApp_Identity_Server>("identity")
    .WithReference(db)
    .WithReference(pgadmin);

var bff = builder.AddProject<Projects.ClinicApp_Identity_BFF>("bff")
    .WithReference(identityServer);

var mainapi = builder.AddProject<Projects.ClinicApp_Presentation>("clinicapp-presentation")
    .WithReference(db)
    .WithReference(pgadmin)
    .WithReference(identityServer)
    .WithReference(bff)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT","Development");

// Apply settings from appsettings.json to the respective projects
ApplyConfigurationToResource(identityServer, builder.Configuration.GetSection("Identity"));
ApplyConfigurationToResource(bff, builder.Configuration.GetSection("BFF"));
ApplyConfigurationToResource(mainapi, builder.Configuration.GetSection("JWT"));

builder.Build().Run();

// Helper method to apply configuration sections to resources as environment variables
static void ApplyConfigurationToResource<T>(IResourceBuilder<T> resourceBuilder, IConfigurationSection section) where T : IResourceWithEnvironment
{
    // If the section has a value, it's a leaf node.
    if (section.Value is not null)
    {
        resourceBuilder.WithEnvironment(section.Path, section.Value);
    }

    // Recurse into children for nested objects and arrays.
    foreach (var child in section.GetChildren())
    {
        ApplyConfigurationToResource(resourceBuilder, child);
    }
}
