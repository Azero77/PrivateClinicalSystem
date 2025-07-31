using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using System.Globalization;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Services.Sessions
{
    public class DoctorScheduler : IScheduler<Doctor>
    {
        private readonly ISessionRepository _repo;
        public DoctorScheduler(ISessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ErrorOr<Created>> CreateSession(Session session,Doctor doctor)
        {

            if (await IsSessionInDoctorSessionsAsync(session, doctor))
                return ScheduleErrors.SessionAlreadyExists;

            return await AddSession(session, doctor);

        }

        private async Task<ErrorOr<Created>> AddSession(Session session, Doctor doctor)
        {
            
            if (await IsSessionInDoctorSessionsAsync(session,doctor))
                return ScheduleErrors.SessionAlreadyExists;
            if (session.DoctorId != doctor.Id)
                return DoctorErrors.DoctorModifyValidationError;

            var isConflictingWithWorkingTime = doctor.CanAddBasedToSchedule(session.SessionDate);
            if (isConflictingWithWorkingTime.IsError)
                return isConflictingWithWorkingTime.Errors;

            //check the session repositories for the new session and check if it overlaps
            var doctorSessions = await _repo.GetFutureSessionsDoctor(doctor);
            var doesOverlaps = IsSessionNotOverlapsWithDoctorSchedule(session, doctorSessions);
            if (doesOverlaps.IsError)
                return doesOverlaps.Errors;
            session.SetSession();
            return Result.Created;
        }
        private async Task<bool> IsSessionInDoctorSessionsAsync(Session session, Doctor doctor)
        {
            var doctorSession = (await _repo.GetAllSessionsForDoctor(doctor)).Select(s => s.Id).ToList();
            if (doctorSession.Contains(session.Id))
                return true;
            return false;
        }
        public async Task<ErrorOr<Deleted>> DeleteSession(Session session, Doctor doctor)
        {
            if (await IsSessionInDoctorSessionsAsync(session, doctor))
                return DoctorErrors.DoctorModifyValidationError;
            await _repo.RemoveSessionFromDoctor(session);
            return Result.Deleted;
        }

        public ErrorOr<Success> PaySession(Session session,Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public ErrorOr<Success> SetSession(Session session,Doctor doctor)
        {
            session.SetSession();
            return Result.Success;
        }

        public async Task<ErrorOr<Updated>> UpdateSession(Session session, TimeRange newTime,Doctor doctor)
        {
            var result = await AddSession(session, doctor);
            if (result.IsError)
                return result.Errors;
            return Result.Updated;
        }

        private static ErrorOr<Success> IsSessionNotOverlapsWithDoctorSchedule(Session newSession,IEnumerable<Session> doctorSessions)
        {
            
            foreach (var doctorsession in doctorSessions)
            {
                if (
                    newSession.Id != doctorsession.Id
                    &&
                    TimeRange.IsOverlapping(doctorsession.SessionDate, newSession.SessionDate)
                   )
                    return ScheduleErrors.ConflictingSession(newSession, doctorsession);
            }
            return Result.Success;
        }
    }
}
