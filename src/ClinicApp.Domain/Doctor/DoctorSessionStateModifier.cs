using ClinicApp.Domain.Services;
using ErrorOr;

namespace ClinicApp.Domain.Doctor;

public class DoctorApprovalSessionStateModifier : ISessionStateModifier
{
    private Doctor _doctor;

    public DoctorApprovalSessionStateModifier(Doctor doctor)
    {
        this._doctor = doctor;
    }
    public ErrorOr<Success> Modify(Session.Session session)
    {
        var result = _doctor.AddSession(session);
        if (result.IsError)
            return result;
        session.SetSession();
        return Result.Success;
    }
}
