namespace ClinicApp.Contracts;

public record GetPresignedUrlResponse(string key, string presignedUrl);