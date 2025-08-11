using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using System.Globalization;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Services.Sessions
{
    public class DoctorScheduler : IScheduler
    {
        private readonly ISessionRepository _repo;
        public DoctorScheduler(ISessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ErrorOr<Created>> CreateSession(Session session,Doctor doctor,IReadOnlyCollection<Session> doctorSessions)
        {
            var result = await AddSession(session, doctor);
            if(!result.IsError)
                await _repo.AddSession(session);
            return result;
        }

        private async Task<ErrorOr<Created>> AddSession(Session session, Doctor doctor)
        {
            if (session.DoctorId != doctor.Id)
                return DoctorErrors.DoctorModifyValidationError;

            var isConflictingWithWorkingTime = doctor.CanAddBasedToSchedule(session.SessionDate);
            if (isConflictingWithWorkingTime.IsError)
                return isConflictingWithWorkingTime.Errors;

            //check the session repositories for the new session and check if it overlaps
            //if session is not in midnight we check for sessions for today only, but if it was we check for sessions for the day and the day after
            IReadOnlyCollection<Session>? doctorSessions = await _repo.GetFutureSessionsDoctor(doctor);

            if (session.SessionDate.IsMidnight)
            {
                doctorSessions = await _repo.GetSesssionsForDoctorOnDayAndAfter(doctor.Id,session.SessionDate.StartTime);
            }
            else
            {
                doctorSessions = await _repo.GetSessionsForDoctorOnDay(doctor.Id, session.SessionDate.StartTime);
            }
            var doesOverlaps = IsSessionNotOverlapsWithDoctorSchedule(session, doctorSessions);
            if (doesOverlaps.IsError)
                return doesOverlaps.Errors;
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
            var result = await _repo.DeleteSession(session.Id);
            return  result is null ? Error.NotFound() : Result.Deleted;
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
