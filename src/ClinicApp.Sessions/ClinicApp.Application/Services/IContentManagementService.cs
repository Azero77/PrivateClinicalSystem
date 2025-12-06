using System.Text.Json;

namespace ClinicApp.Application.Services;
public interface IContentManagementService
{
    Task<JsonElement> FromServerAsync(JsonElement serverJson);
}