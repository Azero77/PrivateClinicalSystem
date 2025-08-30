using Aspire.Hosting;

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

builder.Build().Run();
