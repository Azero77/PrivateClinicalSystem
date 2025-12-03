using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ClinicApp.Application.Services;
/// <summary>
/// Service For dealing with session content for handling the conversion of json content to a readable format by the text editor
/// 1- Conversion between S3 urls to presigned urls for photots and videos
/// </summary>
internal sealed class JsonContentManagementService
{
    private const string s3UrlRegex = @"s3://([^/]+)/(.+)";

    public JsonElement FromClient(JsonElement clientJson)
    {
        throw new Exception();
    }

    /// <summary>
    /// Changing s3 urls to presigned urls
    /// </summary>
    /// <param name="serverJson"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public JsonElement FromServer(JsonElement serverJson)
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
        List<Task> tasks = new();
        JsonNode? json = JsonNode.Parse(serverJson.GetRawText());
        if (json is null)
            throw new NotSupportedException();
        ConvertS3UrlToPresignedUrls(json,tasks);

        return JsonDocument.Parse(json.ToJsonString()).RootElement;
    }

    private static void ConvertS3UrlToPresignedUrls(JsonNode json, List<Task> tasks)
    {
        if (json?["attrs"] is JsonNode attrs
                &&
            attrs?["src"]?.GetValue<string>() is string src)
        {
            var match = Regex.Match(src, s3UrlRegex);

            if (match.Success)
            {
                string bucketName = match.Groups[1].Value;
                string key = match.Groups[2].Value;

                //handle
                attrs["src"] = "presigned url"; //todo
            }
        }

        if (json?["content"] is JsonArray items)
        {
            foreach (var item in items)
            {
                if(item is not null)
                    ConvertS3UrlToPresignedUrls (item,tasks);
            }
        }
    }
}
