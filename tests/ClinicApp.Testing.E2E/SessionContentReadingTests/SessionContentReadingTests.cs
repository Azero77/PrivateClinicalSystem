using API.Services;
using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure.Persistance.Seeding;
using ClinicApp.Infrastructure.Services;
using ClinicApp.Presentation.Tests.IntegrationTest;
using ClinicApp.Resources.Testing.Integration;
using ClinicApp.Shared.QueryTypes;
using FFF.MimeTypes;
using FluentAssertions;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicApp.Testing.E2E.SessionContentReadingTests;
[Collection(nameof(WebFactoryCollectionDefinition))]
public class SessionContentReadingTests
{
    private readonly HttpClient _resourcesApiClient;
    private readonly HttpClient _sessionApiClient;
    public static HttpClient _webClient = new();

    public WebFactory Factory { get; }

    public SessionContentReadingTests(WebFactory factory)
    {
        _resourcesApiClient = factory.ResourcesApiFactory.CreateClient();
        _sessionApiClient = factory.SessionApiFactory.Client;
        Factory = factory;
    }
    [Fact]
    public async Task SessionContent_ShouldReturnPresignedUrlsForEachFile_WhenContentIsValid()
    {
        //assing
        //load the files, load the session object
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles/github_photo.jpg");
        var file = File.Open(filePath,FileMode.Open);
        var PutpresignedUrlResponse = await _resourcesApiClient.PostAsJsonAsync("files/",
            new 
            {
                fileName = Path.GetFileName(file.Name),
                contentType = MimeTypeMap.GetMimeType(Path.GetExtension(file.Name)),
            });
        PutpresignedUrlResponse.IsSuccessStatusCode.Should().Be(true);

        GetPresignedUrlResponse? response = await PutpresignedUrlResponse.Content.ReadFromJsonAsync<GetPresignedUrlResponse>();
        response?.Should().NotBeNull();

        var key = response!.key;
        var presignedUrl = response!.presignedUrl;
       
        // Serialize to JsonElement
        //act
        //upload files to s3
        var putResponse = await _webClient.PutAsync(presignedUrl,new StreamContent(file));
        putResponse.EnsureSuccessStatusCode();

        string fileUrl = JsonContentManagementService.GetS3Url(key,Factory.GetS3Settings().BucketName);
        IClock clock = new TestClock();
        var tiptapDocument = new
        {
            type = "doc",
            content = new[]
   {
        new
        {
            type = "paragraph",
            content = new object[]
            {
                new { type = "text", text = "Here is your uploaded file: " },
                new
                {
                    type = "image",
                    attrs = new { src = fileUrl, alt = "Uploaded file" }
                }
            }
        }
    }
        };
        JsonElement tiptapJsonElement = JsonSerializer.SerializeToElement(tiptapDocument);


        var date = clock.UtcNow.AddDays(10).Date;
        var starttime = new DateTimeOffset(DateOnly.FromDateTime(date), new TimeOnly(10, 30), TimeSpan.FromHours(0));
        var endtime = new DateTimeOffset(DateOnly.FromDateTime(date), new TimeOnly(11, 30), TimeSpan.FromHours(0));
        var addSessionRequest = new AddSessionRequest(
            starttime,
            endtime,
            tiptapJsonElement,
            SeedData.Room1Id,
            SeedData.Patient1Id,
            SeedData.Doctor1Id
            );
        //add new session object to db
        var addSessionResponse = await _sessionApiClient.PostAsJsonAsync<AddSessionRequest>("api/session/add",addSessionRequest);
        addSessionResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var addedSession = await addSessionResponse.Content.ReadFromJsonAsync<SessionDTO>();
        addedSession?.Should()?.NotBeNull();
        //trying to retreive it
        var session= await _sessionApiClient.GetFromJsonAsync<SessionQueryType>($"api/Session/sessions/{addedSession!.Id}");

        //assert
        session?.Should()?.NotBeNull();
        session!.Content.Should().NotBe(default(JsonElement));



        //clean

        (await _sessionApiClient.DeleteAsync($"api/session/{addedSession.Id}"))
            .EnsureSuccessStatusCode();
    }



}
