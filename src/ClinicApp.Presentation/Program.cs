using ClinicApp.Application;
using ClinicApp.Domain;
using ClinicApp.Infrastructure;
using ClinicApp.Infrastructure.Extensions;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace ClinicApp.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(builder.Configuration.GetConnectionString("postgresClinicdb"));
                opts.LogTo(Console.WriteLine, LogLevel.Information);
            });
            builder.Services.AddDomainServices()
                .AddInfrastructure()
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

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }
    }
}
