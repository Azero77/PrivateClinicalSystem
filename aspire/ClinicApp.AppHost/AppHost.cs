using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresClinic = builder.AddPostgres("postgres")
    .WithImage("postgres:16.1-alpine3.19")
    .WithPgAdmin()
    .WithDataVolume(isReadOnly : false);
var db = postgresClinic.AddDatabase("postgresClinicdb");

builder.AddProject<Projects.ClinicApp_Presentation>("clinicapp-presentation")
    .WithReference(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT","Development");

builder.Build().Run();
