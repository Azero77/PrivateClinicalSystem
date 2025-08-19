using ClinicApp.Application;
using ClinicApp.Domain;
using ClinicApp.Infrastructure;
using ClinicApp.Infrastructure.Extensions;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Presentation.Exceptions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ClinicApp.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();

            //Adding handler for unhandled exception thrown
            builder.Services.AddProblemDetails(cnfg =>
            {
                cnfg.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";;
                    context.ProblemDetails.Extensions.TryAdd("requestId",context.HttpContext.TraceIdentifier);
                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.Add("traceId",activity?.TraceId);
                };
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDomainServices()
                .AddInfrastructure(builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("Connection string is null for clinic db"))
                .AddApplication();

            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (app.Environment.IsDevelopment())
            {
                app.RunMigrations();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }
    }
}
