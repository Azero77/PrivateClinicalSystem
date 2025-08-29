using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
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
        //Encapsulated the logic of creating the session only in this service
        public async Task<ErrorOr<Session>> CreateSession(
                        Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       IClock clock,
                       UserRole role,
                       Doctor doctor)
        {
            ErrorOr<Session> session = Session.Schedule(id,
                sessionDate,
                sessionDescription,
                roomId,
                patientId,
                doctorId,
                clock,
                role
                );

            if (session.IsError)
                return session.Errors;
            var canadd =  await CanAddSession(session.Value, doctor);
            if (canadd.IsError)
                return canadd.Errors;
            return await _repo.AddSession(session.Value);
        }

        private async Task<ErrorOr<Created>> CanAddSession(Session session, Doctor doctor)
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
            var result = await CanAddSession(session, doctor);
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
