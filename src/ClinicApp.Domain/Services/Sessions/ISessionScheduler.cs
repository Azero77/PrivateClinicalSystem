using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Services.Sessions
{
    public interface IScheduler
    {
        Task<ErrorOr<Session>> CreateSession(
                        Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid? patientId,
                       Guid doctorId,
                       IClock clock,
                       UserRole role,
                       Doctor doctor);
        ErrorOr<Success> SetSession(Session session, Doctor doctor);
        Task<ErrorOr<Updated>> UpdateSession(Session session, TimeRange newTime, Doctor doctor);
        Task<ErrorOr<Deleted>> DeleteSession(Session session, Doctor doctor);
        ErrorOr<Success> PaySession(Session session, Doctor doctor);
    }
}
