using ClinicApp.Application;
using ClinicApp.Domain;
using ClinicApp.Infrastructure;
using ClinicApp.Infrastructure.Extensions;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.Seeding;
using ClinicApp.Presentation.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using System.Diagnostics;
using System.Text;

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
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                    context.ProblemDetails.Extensions.Add("traceId", System.Diagnostics.Activity.Current?.TraceId.ToHexString());
                };
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtoptions =>
                {

                    //a key will be used in dev and ignoring authority claim
                    //but in prod we will not use symmetric keys and we will move to assymetric
                    //in prod we don't need to provide key because while providng authority claim it will check .well-know/configuration to get the public key to validate issued token
                    //without the need to know the private key
                    
                    if (builder.Environment.IsDevelopment())
                    {
                        jwtoptions.RequireHttpsMetadata = false;
                        SymmetricSecurityKey key = getKey(builder);

                        jwtoptions.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidAudience = builder.Configuration?["JWT:Audience"] ?? throw new ArgumentException("IdentityUrl Should Be Provided"),
                            ValidIssuer = builder.Configuration?["JWT:Authority"] ?? throw new ArgumentException("IdentityUrl Should Be Provided"),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            IssuerSigningKey = key
                        };
                    }
                    else
                    {
                        jwtoptions.Authority = builder.Configuration?["JWT:Authority"] ?? throw new ArgumentException("IdentityUrl Should Be Provided");
                        jwtoptions.Audience = builder.Configuration?["JWT:Audience"] ?? throw new ArgumentException("IdentityUrl Should Be Provided");
                        jwtoptions.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true
                        };
                    }
                    jwtoptions.MapInboundClaims = false;

                    static SymmetricSecurityKey getKey(WebApplicationBuilder builder)
                    {
                        // This secret must be the base64 encoded key from 'dotnet user-jwts'
                        var key_string = builder.Configuration?["JWT:Key"] ?? throw new ArgumentException("JWT:Key was not provided");
                        var bytes = Convert.FromBase64String(key_string);
                        var key = new SymmetricSecurityKey(bytes);
                        return key;
                    }
                });

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly,includeInternalTypes:true);

            AddSerilog(builder);
            builder.Services.AddAuthorization();
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



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (app.Environment.IsDevelopment())
            {
                app.RunMigrations();
                app.SeedDataInDevelopment();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }

        private static void AddSerilog(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
        }
    }
}
