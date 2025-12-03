
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using API.Models;
using API.Services;
using ClinicApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
        builder.Services.Configure<S3Settings>(builder.Configuration.GetSection(nameof(S3Settings)));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region),
                ServiceURL = s3Settings.ServiceUrl,
                ForcePathStyle = s3Settings.ForcePathStyle
                
            };
            return new AmazonS3Client(config);
        });
        builder.Services.AddSingleton<IUploadService, S3UploadService>();
        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        if (app.Environment.IsDevelopment())
        {
            app.MapPost("upload", async (IFormFile file,
                                     [FromServices] IAmazonS3 client,
                                     [FromServices] IOptions<S3Settings> s3Settings) =>
            {
                if (file.Length == 0)
                    return Results.BadRequest("file cannot be empty");

                Guid key = Guid.NewGuid();
                using Stream stream = file.OpenReadStream();
                var request = new PutObjectRequest()
                {
                    BucketName = s3Settings.Value.BucketName,
                    ContentType = file.ContentType,
                    Metadata =
                {
                    ["file-name"] = file.FileName
                },
                    Key = $"files/{key}",
                    InputStream = stream
                };
                await client.PutObjectAsync(request);

                return Results.Ok(key);
            }).DisableAntiforgery();
        }

        app.MapGet("files/{key}" , async (string key,IUploadService uploadService) =>
        {
            var result = await uploadService.GetFile(key);
            return result.Match(value => Results.Ok(value),error => ProblemResult.Create(error));
        });

        app.MapPost("files/", async (string fileName,string contentType,IUploadService uploadService) =>
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(contentType))
            {
                return Results.BadRequest();
            }
            var result = await uploadService.UploadFile(fileName,contentType);
            return result.Match(value => Results.Ok(value),error => ProblemResult.Create(error));
        });

        app.MapGet("files/list", async (string keys, IUploadService uploadService) =>
        {
            List<string> keysAsList = keys.Split("-").ToList();

            var result = await uploadService.GetFiles(keysAsList);
            return Results.Ok(result);
        });

        app.MapDelete("files/{key:string}", async(string key, IUploadService uploadService) =>
        {
            var response = await uploadService.DeleteFile(key);

            return response.Match(value => Results.Ok(value), error => ProblemResult.Create(error));
        });
        app.Run();
    }
}
