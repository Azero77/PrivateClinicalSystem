using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Session;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Doctor
{
    public class Doctor : AggregateRoot
    {
        public Doctor(Guid id) : base(id) { }
        public List<Guid> SessionIds { get; private set; } = new();
        public WorkingDays WorkingDays { get; private set; }
        public WorkingHours WorkingHours { get; private set; }
        public ErrorOr<Success> AddSession(Session.Session session)
        {
            if (this.SessionIds.Contains(session.Id))
                return Error.Validation("Doctor.Validation", "Can't Add the session, it is already Added");

            if (session.DoctorId != this.Id)
                return DoctorErrors.DoctorModifyValidationError;
            var canAdd = SessionConflictsWithDoctor(session);
            if (canAdd.IsError)
            {
                return canAdd.Errors;
            }
            SessionIds.Add(session.Id);
            return Result.Success;
        }

        internal ErrorOr<Success> SessionConflictsWithDoctor(Session.Session session)
        {
            WorkingDays sessionDay = (WorkingDays)(1 << ((int)session.SessionDate.StartTime.DayOfWeek));

            if (SessionConflictsWithWorkingDays(sessionDay))
            {
                return DoctorErrors.SessionOutOfWorkingDay(session.SessionDate.StartTime.DayOfWeek, this.WorkingDays);
            }
            else if (SessionConflictsWithWorkingHours(session))
            {
                return DoctorErrors.SessionOutOfWorkingHours(session.SessionDate, this.WorkingHours);
            }
            return Result.Success;
        }

        internal void RemoveSession(Guid sessionId)
        {
            if(SessionIds.Contains(sessionId))
                this.SessionIds.Remove(sessionId);
        }

        private bool SessionConflictsWithWorkingDays(WorkingDays sessionDay)
        {
            return (sessionDay & WorkingDays) != sessionDay;
        }

        private bool SessionConflictsWithWorkingHours(Session.Session session)
        {
            return !(
                            TimeOnly.FromDateTime(session.SessionDate.StartTime) > WorkingHours.StartTime &&
                            TimeOnly.FromDateTime(session.SessionDate.EndTime) < WorkingHours.EndTime);
        }

    }

    public record WorkingHours
    {
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        internal bool InDayLight => StartTime < EndTime; //To check if a doctor has midnight working hours (very rare)
    }
}
