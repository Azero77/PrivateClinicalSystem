using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Doctor;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Session;
using ErrorOr;
using System.Globalization;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Services.Sessions
{
    public class DoctorSessionScheduler : ISessionScheduler<Doctor.Doctor>
    {
        private readonly ISessionRepository _repo;
        public DoctorSessionScheduler(ISessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ErrorOr<Created>> CreateSession(Session.Session session,Doctor.Doctor doctor)
        {

            if (await IsSessionInDoctorSessionsAsync(session, doctor))
                return Error.Validation("Doctor.Validation", "Can't Add the session, it is already Added");

            return await AddSession(session, doctor);

        }

        private async Task<ErrorOr<Created>> AddSession(Session.Session session, Doctor.Doctor doctor)
        {
            
            if (await IsSessionInDoctorSessionsAsync(session,doctor))
                return Error.Validation("Doctor.Validation", "Can't Add the session, it is already Added");
            if (session.DoctorId != doctor.Id)
                return DoctorErrors.DoctorModifyValidationError;

            var isConflictingWithWorkingTime = doctor.SessionConflictsWithDoctor(session.SessionDate);
            if (isConflictingWithWorkingTime.IsError)
                return isConflictingWithWorkingTime.Errors;

            //check the session repositories for the new session and check if it overlaps
            var doctorSessions = await _repo.GetFutureSessionsDoctor(doctor);
            var doesOverlaps = IsSessionOverlapsWithDoctorSchedule(session, doctorSessions);
            if (doesOverlaps.IsError)
                return doesOverlaps.Errors;
            session.SetSession();
            return Result.Created;
        }
        private async Task<bool> IsSessionInDoctorSessionsAsync(Session.Session session, Doctor.Doctor doctor)
        {
            var doctorSession = (await _repo.GetAllSessionsForDoctor(doctor)).Select(s => s.Id).ToList();
            if (doctorSession.Contains(session.Id))
                return true;
            return false;
        }
        public async Task<ErrorOr<Deleted>> DeleteSession(Session.Session session, Doctor.Doctor doctor)
        {
            if (await IsSessionInDoctorSessionsAsync(session, doctor))
                return DoctorErrors.DoctorModifyValidationError;
            await _repo.RemoveSessionFromDoctor(session);
            return Result.Deleted;
        }

        public ErrorOr<Success> PaySession(Session.Session session,Doctor.Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public ErrorOr<Success> SetSession(Session.Session session,Doctor.Doctor doctor)
        {
            session.SetSession();
            return Result.Success;
        }

        public async Task<ErrorOr<Updated>> UpdateSession(Session.Session session, TimeRange newTime,Doctor.Doctor doctor)
        {
            var result = await AddSession(session, doctor);
            if (result.IsError)
                return result.Errors;
            return Result.Updated;
        }

        private static ErrorOr<Success> IsSessionOverlapsWithDoctorSchedule(Session.Session newSession,IEnumerable<Session.Session> doctorSessions)
        {
            
            foreach (var doctorsession in doctorSessions)
            {
                if (
                    newSession.Id != doctorsession.Id
                    &&
                    newSession.SessionDate.StartTime < doctorsession.SessionDate.EndTime
                    && 
                    doctorsession.SessionDate.StartTime < newSession.SessionDate.EndTime
                    )
                    return Error.Validation("Schedule.Conflict", $"Can't Book this time", new Dictionary<string, object>()
                    {
                        {"Conflicted",newSession},
                        {"Exisiting",doctorsession}
                    });
            }
            return Result.Success;

        }
    }
}
