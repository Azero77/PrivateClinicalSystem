namespace ClinicApp.Identity.Server.ConfigurationModels;

public class ExternalAuthProvider
{
    public string ProviderName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
