using ClinicApp.Domain.Common;

namespace ClinicApp.Identity.Server.Services;

public class DomainUserRegisterContext
{
    public UserRole SelectedRole { get; set; }
    public string content { get; set; } = string.Empty;
}