namespace API.Models;

public class S3Settings
{
    public string Region { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string ServiceUrl { get; set; } = null!;
    public bool ForcePathStyle { get; set; }
}
