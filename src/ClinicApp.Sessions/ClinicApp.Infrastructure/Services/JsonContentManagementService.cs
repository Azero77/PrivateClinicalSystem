using ClinicApp.Application.Services;
using ClinicApp.Contracts;
using ErrorOr;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ClinicApp.Infrastructure.Services;
/// <summary>
/// Service For dealing with session content for handling the conversion of json content to a readable format by the text editor
/// 1- Conversion between S3 urls to presigned urls for photots and videos
/// </summary>
internal sealed class JsonContentManagementService : IContentManagementService
{
    private const string s3UrlRegex = @"s3://([^/]+)/(.+)";
    private readonly IResourcesClientService _client;

    public JsonContentManagementService(IResourcesClientService client)
    {
        _client = client;
    }
    /// <summary>
    /// Changing s3 urls to presigned urls
    /// </summary>
    /// <param name="serverJson"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<JsonElement> FromServerAsync(JsonElement serverJson)
    {

        //to know that an image from an s3 is found to re{place
        //each src that has s3://{bucket-name}/{url} -> presigned url

        //The json will have the following structure:
        /*
    {
      "type": "doc",
      "content": [
        {
          "type": "paragraph",
          "content": [
            { "type": "text", "text": "Session description..." }
          ]
        },
        {
          "type": "image",
          "attrs": {
            "src": "s3://session-content-bucket/1234et4342"
          }
        }
      ]
    }
         */
        JsonNode? json = JsonNode.Parse(serverJson.GetRawText());
        if (json is null)
            throw new NotSupportedException();
        var keys = AssignKeysToList(json);
        var keysPresignedUrls = await _client.GetPreSignedUrls(keys);
        AssignPresignedUrlsToJson(json, keysPresignedUrls);
        return JsonDocument.Parse(json.ToJsonString()).RootElement;
    }


    private static List<string> AssignKeysToList(JsonNode json)
    {
        List<string> keys = new();
        ExtractSrcFromContentObject(json, keys);

        if (json?["content"] is JsonArray items)
        {
            foreach (var item in items)
            {
                if (item is not null)
                    ExtractSrcFromContentObject(json, keys);
            }
        }
        return keys;

        static void ExtractSrcFromContentObject(JsonNode json, List<string> keys)
        {
            if (json?["attrs"] is JsonNode attrs
                            &&
                        attrs?["src"]?.GetValue<string>() is string src)
            {
                var match = Regex.Match(src, s3UrlRegex);

                if (match.Success)
                {
                    string key = match.Groups[2].Value;
                    keys.Add(key);
                }
            }
        }
    }

    private static List<string> AssignPresignedUrlsToJson(JsonNode json, List<GetPresignedUrlResponse> dictionary)
    {
        List<string> keys = new();
        ExtractSrcFromContentObject(json, dictionary);
        if (json?["content"] is JsonArray items)
        {
            foreach (var item in items)
            {
                if (item is not null)
                    ExtractSrcFromContentObject(json, dictionary);
            }
        }
        return keys;

        static void ExtractSrcFromContentObject(JsonNode json, List<GetPresignedUrlResponse> dictionary)
        {
            if (json?["attrs"] is JsonNode attrs
                            &&
                        attrs?["src"]?.GetValue<string>() is string src)
            {
                var match = Regex.Match(src, s3UrlRegex);

                if (match.Success)
                {
                    string key = match.Groups[2].Value;
                    string presignedUrl = dictionary.FirstOrDefault(i => i.key == key)?.presignedUrl ?? ""; //404 image not found

                    attrs["src"] = presignedUrl;
                }
            }
        }
    }
}

public interface IResourcesClientService
{
    /// <summary>
    /// Get the presigned Urls for keys 
    /// </summary>
    /// <param name="keys"></param>
    /// <returns>A dictionary where Key of the item is the key and the value is the presigned url</returns>
    Task<List<GetPresignedUrlResponse>> GetPreSignedUrls(List<string> keys, CancellationToken token = default);
}

public class HttpResourcesClientService : IResourcesClientService
{
    private readonly HttpClient _client;
    public const string HttpResourceClientServiceClientName = "ResourcesClient";

    public HttpResourcesClientService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(HttpResourceClientServiceClientName);
    }

    public async Task<List<GetPresignedUrlResponse>> GetPreSignedUrls(List<string> keys, CancellationToken token = default)
    {
        string joinedKeys = string.Concat(keys);
        HttpResponseMessage response = await _client.GetAsync($"files/list?keys={joinedKeys}",token);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<GetPresignedUrlResponse>().ToList();
        }

        var body = await response.Content.ReadFromJsonAsync<List<GetPresignedUrlResponse>>(token);
        return body ?? new() ;
    }
}