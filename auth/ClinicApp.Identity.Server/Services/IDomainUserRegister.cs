using ClinicApp.Identity.Server.Infrastructure.Persistance;

namespace ClinicApp.Identity.Server.Services;


/// <summary>
/// Interface for the user to add domain services while complete registeration
/// </summary>
public interface IDomainUserRegister
{
    Task Modify(ApplicationUser user,DomainUserRegisterContext context);
}
