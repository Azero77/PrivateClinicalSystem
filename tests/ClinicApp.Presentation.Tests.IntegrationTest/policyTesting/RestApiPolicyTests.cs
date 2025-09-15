
using System.Net;
using System.Net.Http.Json;
using ClinicApp.Presentation.Requests;
using FluentAssertions;

namespace ClinicApp.Presentation.Tests.IntegrationTest.policyTesting;

[Collection(ApiFactory.ApiCollectionTests)]
public class RestApiPolicyTests
{
    private readonly ApiFactory _apiFactory;

    public RestApiPolicyTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    private HttpClient GetClientForRole(string role)
    {
        var token = Environment.GetEnvironmentVariable($"Test_ClinicApp_{role}Token");
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException($"JWT token for role '{role}' not found. Please set the 'Test_ClinicApp_{role}Token' environment variable.");
        }
        var client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Theory]
    [InlineData("Admin", HttpStatusCode.OK)]
    [InlineData("Doctor", HttpStatusCode.Forbidden)]
    [InlineData("Patient", HttpStatusCode.Forbidden)]
    public async Task GetRooms_ShouldReturnExpectedStatusCode_ForRole(string role, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var client = GetClientForRole(role);

        // Act
        var response = await client.GetAsync("/api/rooms");

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [InlineData("Admin", HttpStatusCode.Created)]
    [InlineData("Doctor", HttpStatusCode.Forbidden)]
    [InlineData("Patient", HttpStatusCode.Forbidden)]
    public async Task CreateRoom_ShouldReturnExpectedStatusCode_ForRole(string role, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var client = GetClientForRole(role);
        var request = new CreateRoomRequest("New Test Room From Integration Test");

        // Act
        var response = await client.PostAsJsonAsync("/api/rooms", request);

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [InlineData("Admin", HttpStatusCode.NoContent)]
    [InlineData("Doctor", HttpStatusCode.Forbidden)]
    [InlineData("Patient", HttpStatusCode.Forbidden)]
    public async Task DeleteRoom_ShouldReturnExpectedStatusCode_ForRole(string role, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var client = GetClientForRole(role);
        var roomId = Guid.NewGuid(); 

        // Act
        var response = await client.DeleteAsync($"/api/rooms/{roomId}");

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }
    
    [Theory]
    [InlineData("Admin", HttpStatusCode.OK)]
    [InlineData("Doctor", HttpStatusCode.Forbidden)]
    [InlineData("Patient", HttpStatusCode.Forbidden)]
    public async Task GetSecretaries_ShouldReturnExpectedStatusCode_ForRole(string role, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var client = GetClientForRole(role);

        // Act
        var response = await client.GetAsync("/api/secretaries");

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }
}
