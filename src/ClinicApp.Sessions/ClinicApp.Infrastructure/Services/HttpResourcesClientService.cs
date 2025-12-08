using ClinicApp.Contracts;
using System.Net.Http.Json;

namespace ClinicApp.Infrastructure.Services;

public class HttpResourcesClientService : IResourcesClientService
{
    private readonly HttpClient _client;
    public const string HttpResourceClientServiceClientName = "ResourcesClient";

    public HttpResourcesClientService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<GetPresignedUrlResponse>> GetPreSignedUrls(List<string> keys, CancellationToken token = default)
    {
        string joinedKeys = string.Join(",",keys);
        HttpResponseMessage response = await _client.GetAsync($"files/list?keys={joinedKeys}",token);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<GetPresignedUrlResponse>().ToList();
        }

        var body = await response.Content.ReadFromJsonAsync<List<GetPresignedUrlResponse>>(token);
        return body ?? new() ;
    }
}