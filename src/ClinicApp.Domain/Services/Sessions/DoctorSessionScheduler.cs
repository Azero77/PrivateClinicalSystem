using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Doctor;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Session;
using ErrorOr;

namespace ClinicApp.Domain.Services.Sessions
{
    public class DoctorSessionScheduler : ISessionScheduler<Doctor.Doctor>
    {
        private readonly ISessionRepository _repo;
        public DoctorSessionScheduler(ISessionRepository repo)
        {
            _repo = repo;
        }

        public ErrorOr<Created> CreateSession(Session.Session session,Doctor.Doctor doctor)
        {

            if (doctor.SessionIds.Contains(session.Id))
                return Error.Validation("Doctor.Validation", "Can't Add the session, it is already Added");

            if (session.DoctorId != doctor.Id)
                return DoctorErrors.DoctorModifyValidationError;

            var isConflictingWithWorkingTime = doctor.SessionConflictsWithDoctor(session);
            if (isConflictingWithWorkingTime.IsError)
                return isConflictingWithWorkingTime.Errors;

            //check the session repositories for the new session and check if it overlaps
            
            session.SetSession();
            return Result.Created;


        }

        public ErrorOr<Deleted> DeleteSession(Session.Session session, Doctor.Doctor doctor)
        {
            if (doctor.SessionIds.Contains(session.Id))
                return DoctorErrors.DoctorModifyValidationError;

            doctor.RemoveSession(session.Id);
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

        public ErrorOr<Updated> UpdateSession(Session.Session session, TimeRange newTime,Doctor.Doctor doctor)
        {

        }

    }
}
