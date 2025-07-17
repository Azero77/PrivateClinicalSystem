using ClinicApp.Domain.Session;
using ErrorOr;

namespace ClinicApp.Domain.Services;

public interface ISessionStateModifier
{
    public ErrorOr<Success> Modify(Session.Session session); 
}
