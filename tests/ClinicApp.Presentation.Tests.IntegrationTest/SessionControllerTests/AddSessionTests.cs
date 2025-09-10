using Bogus;
using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
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
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ClinicApp.Presentation.Tests.IntegrationTest.SessionControllerTests;


[Collection(ApiFactory.ApiCollectionTests)]
public class AddSessionTests
{
    private readonly ApiFactory _api;
    private readonly HttpClient _client;
    private readonly IClock _clock;
    private readonly ITestOutputHelper _testOutputHelper;
    public AddSessionTests(ApiFactory api, ITestOutputHelper testOutputHelper)
    {
        _api = api;
        _client = api.Client;
        _testOutputHelper = testOutputHelper;
        _clock = TestClock.Clock();
    }


    [Fact]
    public async Task AddSession_ShouldReturnCreated_WhenAddIsValid()
    {
        var date = _clock.UtcNow.AddDays(2).Date;
        var starttime = new DateTimeOffset(DateOnly.FromDateTime(date), new TimeOnly(10, 30),TimeSpan.FromHours(0));
        var endtime = new DateTimeOffset(DateOnly.FromDateTime(date), new TimeOnly(11, 30),TimeSpan.FromHours(0));
        var addSessionRequest = new AddSessionRequest(
            starttime,
            endtime,
            new SessionDescription("Test content"),
            SeedData.Room1Id,
            SeedData.Patient1Id,
            SeedData.Doctor1Id
            );
        //Act

        var result = await _client.PostAsJsonAsync<AddSessionRequest>("/Add",addSessionRequest);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact]
    public async Task AddSession_ShouldReturnErrorConflict_WhenSessionConflicts()
    {
        //Arrange
        var alreadyExistedSession = SeedData.Sessions(_clock).First();
        //will add the same session from the test before to check conflicts
        var addSessionRequest = new AddSessionRequest(
            alreadyExistedSession.SessionDate.StartTime,
            alreadyExistedSession.SessionDate.EndTime,
            new SessionDescription("Test content"),
            SeedData.Room1Id,
            SeedData.Patient1Id,
            SeedData.Doctor1Id
            );
        //Act

        var result = await _client.PostAsJsonAsync<AddSessionRequest>("/Add", addSessionRequest);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.IsSuccessStatusCode.Should().Be(false);
        var problemdetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        problemdetails.Should().NotBeNull();
        var errors = problemdetails.Extensions?["Errors"];
        errors.Should().NotBeNull();
        var json = (JsonElement) errors;
        var element = json[0].GetProperty("code").ToString().Should().Be(ScheduleErrors.ConflictingSessionCode);
    }


}
