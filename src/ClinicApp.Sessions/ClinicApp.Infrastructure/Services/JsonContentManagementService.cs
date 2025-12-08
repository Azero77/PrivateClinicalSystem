using ClinicApp.Application.Services;
using ClinicApp.Contracts;
using ErrorOr;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ClinicApp.Infrastructure.Services;
/// <summary>
/// Service For dealing with session content for handling the conversion of json content to a readable format by the text editor
/// 1- Conversion between S3 urls to presigned urls for photots and videos
/// </summary>
public sealed class JsonContentManagementService : IContentManagementService
{
    private const string s3UrlRegex = @"s3://([^/]+)/(.+)";
    private readonly IResourcesClientService _client;

    public static string GetS3Url(string key, string bucketName)
    {
        return $"s3://{bucketName}/{key}";
    }
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


    private static List<string> AssignKeysToList(JsonNode root)
    {
        List<string> keys = new();
        LoopOnJson(root,(attrNode) =>
        {
            string src = attrNode!["src"]!.GetValue<string>();
            var match = Regex.Match(src, s3UrlRegex);

            if (match.Success)
            {
                string key = match.Groups[2].Value;
                keys.Add(key);
            }
        });
        return keys;
    }

    private static void AssignPresignedUrlsToJson(JsonNode root, List<GetPresignedUrlResponse?> getPresignedUrls)
    {
        LoopOnJson(root,(attrNode) =>
        {
            string src = attrNode!["src"]!.GetValue<string>();
            var match = Regex.Match(src, s3UrlRegex);

            if (match.Success)
            {
                string key = match.Groups[2].Value;
                string presignedUrl = getPresignedUrls.FirstOrDefault(i => i.key == key)?.presignedUrl ?? ""; //404 image not found

                attrNode["src"] = JsonValue.Create(presignedUrl);
            }
        });


    }
    private static void LoopOnJson(JsonNode node,Action<JsonNode> attrAction)
    {
        if (node?["content"] is JsonArray items)
        {
            foreach (var item in items)
            {
                if(item is not null)
                    LoopOnJson(item, attrAction);
            }
        }
        else if (node?["attrs"] is JsonNode attrs && 
            attrs["src"] is not null)
        {
            attrAction(attrs);
        }

    }

}
