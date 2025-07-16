var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ClinicApp_Presentation>("clinicapp-presentation");

builder.Build().Run();
