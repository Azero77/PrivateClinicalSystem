using Amazon.S3;
using Amazon.S3.Model;
using API.Models;
using ErrorOr;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace API.Services;

public interface IUploadService
{
    Task<ErrorOr<GetPresignedUrlResponse>> GetFile(string key);
    Task<List<ErrorOr<GetPresignedUrlResponse>>> GetFiles(List<string> keysAsList);
    Task<ErrorOr<GetPresignedUrlResponse>> UploadFile(string fileName, string contentType);
    Task<ErrorOr<Success>> DeleteFile(string key);
}

public class S3UploadService : IUploadService
{

    private readonly IAmazonS3 _client;
    private readonly S3Settings _s3Settings;

    public S3UploadService(IAmazonS3 client, IOptions<S3Settings> s3Settings)
    {
        _client = client;
        _s3Settings = s3Settings.Value;
    }

    public async Task<ErrorOr<GetPresignedUrlResponse>> GetFile(string key)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _s3Settings.BucketName,
                Key = $"files/{key}",
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(15),
            };
            var result = await _client.GetPreSignedURLAsync(request);
            return new GetPresignedUrlResponse(key, result);
        }
        catch (AmazonS3Exception exception)
        {
            return Error.Failure("Resources.Failure", exception.Message);
        }
    }

    public async Task<List<ErrorOr<GetPresignedUrlResponse>>> GetFiles(List<string> keysAsList)
    {
        List<ErrorOr<GetPresignedUrlResponse>> result = new();
        foreach (var key in keysAsList)
        {
             var key_result = await GetFile(key);
            result.Add(key_result);
        }

        return result;
    }

    public async Task<ErrorOr<GetPresignedUrlResponse>> UploadFile(string fileName, string contentType)
    {
        try
        {
            Guid key = Guid.NewGuid();
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _s3Settings.BucketName,
                ContentType = contentType,
                Metadata = {
                        ["file-name"] = fileName
                    },
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Key = $"files/{key}"
            };

            string url = await _client.GetPreSignedURLAsync(request);

            return new GetPresignedUrlResponse(key.ToString(),url);
        }
        catch (AmazonS3Exception exception)
        {
            return Error.Failure("Resources.Failure", exception.Message);
        }
    }
    public async Task<ErrorOr<Success>> DeleteFile(string key)
    {
        DeleteObjectRequest request = new DeleteObjectRequest()
        {
            BucketName = _s3Settings.BucketName,
            Key = key,
        };
        var response = await _client.DeleteObjectAsync(request);
        int code = (int) response.HttpStatusCode;
        return code >= 200 && code < 300 ? Result.Success : Error.Failure();
    }
}

public record GetPresignedUrlResponse(string key, string presignedUrl);