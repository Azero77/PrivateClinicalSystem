using Bogus;
using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services.Sessions;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.Seeding;
using ErrorOr;
using FluentAssertions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Prng;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicApp.Presentation.Tests.IntegrationTest.SessionControllerTests;


[Collection(ApiFactory.ApiCollectionTests)]
public class AddSessionTests
{
    private readonly ApiFactory _api;
    private readonly HttpClient _client;
    public AddSessionTests(ApiFactory api)
    {
        _api = api;
        _client = api.Client;
    }


    [Fact]
    public async Task AddSession_ShouldReturnCreated_WhenAddIsValid()
    {
        //Arrange
        var sessionCommand = new AddSessionCommand(
            TimeRange.Create(
                startTime: DateTimeOffset.UtcNow.AddDays(1).AddHours(10),
                endTime: DateTimeOffset.UtcNow.AddDays(1).AddHours(11)
                ).Value,
            new SessionDescription("Test content"),
            SeedData.Room1Id,
            SeedData.Patient1Id,
            SeedData.Doctor1Id,
            UserRole.Admin
            );
        //Act

        var result = await _client.PostAsJsonAsync<AddSessionCommand>("/Add",sessionCommand);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact]
    public async Task AddSession_ShouldReturnErrorConflict_WhenSessionConflicts()
    {
        //Arrange

        //will add the same session from the test before to check conflicts
        var sessionCommand = new AddSessionCommand(
            TimeRange.Create(
                startTime: DateTimeOffset.UtcNow.AddDays(1).AddHours(10),
                endTime: DateTimeOffset.UtcNow.AddDays(1).AddHours(11)
                ).Value,
            new SessionDescription("Test content"),
            SeedData.Room1Id,
            SeedData.Patient1Id,
            SeedData.Doctor1Id,
            UserRole.Admin
            );
        //Act

        var result = await _client.PostAsJsonAsync<AddSessionCommand>("/Add", sessionCommand);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.IsSuccessStatusCode.Should().Be(false);
        var problemdetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        problemdetails.Should().NotBeNull();
        var errors = problemdetails.Extensions?["Errors"];
        errors.Should().NotBeNull();
        var json = (JsonElement) errors;
        var listerrors = JsonSerializer.Deserialize<List<Error>>(json.GetRawText());
        listerrors.Should().NotBeNull();
        listerrors.First().Code.Should().Be(ScheduleErrors.ConflictingSessionCode);
    }


}
