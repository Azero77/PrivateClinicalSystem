using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services;
using ErrorOr;

namespace ClinicApp.Domain.Doctor;

public class DoctorSessionStateModifier : ISessionStateModifier
{
    private Doctor _doctor;

    public DoctorSessionStateModifier(Doctor doctor)
    {
        _doctor = doctor;
    }

    public ErrorOr<Created> CreateSession(Session.Session session)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<Deleted> DeleteSession(Session.Session session)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<Success> Modify(Session.Session session)
    {
        var result = _doctor.AddSession(session);
        if (result.IsError)
            return result;
        return Result.Success;
    }

    public ErrorOr<Success> PaySession(Session.Session session)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<Success> SetSession(Session.Session session)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<Updated> UpdateSession(Session.Session session, TimeRange newTime)
    {
        throw new NotImplementedException();
    }
}
