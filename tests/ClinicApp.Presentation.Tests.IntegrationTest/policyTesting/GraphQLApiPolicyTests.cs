
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClinicApp.Infrastructure.Persistance.Seeding;
using FluentAssertions;

namespace ClinicApp.Presentation.Tests.IntegrationTest.policyTesting;

[Collection(ApiFactory.ApiCollectionTests)]
public class GraphQLApiPolicyTests
{
    private readonly ApiFactory _apiFactory;

    public GraphQLApiPolicyTests(ApiFactory apiFactory)
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

    private record GraphQLQueryRequest(string Query);
    private record GraphQLResponse(JsonElement Data, JsonElement Errors);

    [Fact]
    public async Task GetSessions_ForDoctor_ShouldReturnOnlyOwnedSessions()
    {
        // Arrange
        var client = GetClientForRole("Doctor");
        var query = new GraphQLQueryRequest("{ sessions { nodes { doctorId } } }");

        // Act
        var response = await client.PostAsJsonAsync("/graphql", query);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GraphQLResponse>();
        content.Errors.ValueKind.Should().Be(JsonValueKind.Undefined);
        var sessions = content.Data.GetProperty("sessions").GetProperty("nodes").EnumerateArray();
        sessions.Should().NotBeEmpty();
        sessions.Should().OnlyContain(s => s.GetProperty("doctorId").GetGuid() == SeedData.Doctor1Id);
    }

    [Fact]
    public async Task GetSessions_ForPatient_ShouldReturnOnlyOwnedSessions()
    {
        // Arrange
        var client = GetClientForRole("Patient");
        var query = new GraphQLQueryRequest("{ sessions { nodes { patientId } } }");

        // Act
        var response = await client.PostAsJsonAsync("/graphql", query);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GraphQLResponse>();
        content.Errors.ValueKind.Should().Be(JsonValueKind.Undefined);
        var sessions = content.Data.GetProperty("sessions").GetProperty("nodes").EnumerateArray();
        sessions.Should().NotBeEmpty();
        sessions.Should().OnlyContain(s => s.GetProperty("patientId").GetGuid() == SeedData.Patient1Id);
    }

    [Fact]
    public async Task GetSessions_ForAdmin_ShouldReturnAllSessions()
    {
        // Arrange
        var client = GetClientForRole("Admin");
        var query = new GraphQLQueryRequest("{ sessions { nodes { id } } }");

        // Act
        var response = await client.PostAsJsonAsync("/graphql", query);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GraphQLResponse>();
        content.Errors.ValueKind.Should().Be(JsonValueKind.Undefined);
        var sessions = content.Data.GetProperty("sessions").GetProperty("nodes").EnumerateArray();
        sessions.Should().NotBeEmpty();
        // We expect to see at least the seeded session
        sessions.Should().Contain(s => s.GetProperty("id").GetGuid() == SeedData.Session1Id);
    }
}
