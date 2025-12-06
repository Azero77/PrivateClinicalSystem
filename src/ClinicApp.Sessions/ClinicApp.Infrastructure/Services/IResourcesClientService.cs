using ClinicApp.Contracts;

namespace ClinicApp.Infrastructure.Services;

public interface IResourcesClientService
{
    /// <summary>
    /// Get the presigned Urls for keys 
    /// </summary>
    /// <param name="keys"></param>
    /// <returns>A dictionary where Key of the item is the key and the value is the presigned url</returns>
    Task<List<GetPresignedUrlResponse>> GetPreSignedUrls(List<string> keys, CancellationToken token = default);
}
