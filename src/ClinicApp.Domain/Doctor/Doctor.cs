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
        public Schedule _schedule { get; private set; } = null!;

        public List<Guid> _sessionIds { get; private set; } = new();
        public WorkingDays WorkingDays { get; private set; }
        public WorkingHours WorkingHours { get; private set; }
        public ErrorOr<Success> AddSession(Session.Session session)
        {
            if (session.DoctorId != this.Id)
                throw new ArgumentException("Can't Modify the session of another doctor");
            WorkingDays sessionDay = (WorkingDays)(1 << ((int)session.SessionDate.StartTime.DayOfWeek));
            if (SessionConflictsWithWorkingDays(sessionDay))
            {
                return Error.Conflict(DoctorErrors.SessionOutOfWorkingDay,
                    "Session Out of working Days of the doctor",
                    new Dictionary<string, object> {
                        { "SessionDay" , session.SessionDate.StartTime.DayOfWeek.ToString() },
                        { "DoctorDays" , Enum.GetValues<WorkingDays>().Where(d => d != WorkingDays.None && d.HasFlag(WorkingDays)).ToList().Select(d => d.ToString())}
                    });
            }
            else if (SessionConflictsWithWorkingHours(session))
            {
                return Error.Conflict(DoctorErrors.SessionOutOfWorkingDay,
                    "Session Out of working hours of the doctor",
                    new Dictionary<string, object>
                    {
                        { "SessionTime" , session.SessionDate},
                        { "DoctorTime" , this.WorkingHours }
                    });
            }
            var result = _schedule.CanBookTimeSlot(session.SessionDate);
            if (result.IsError)
                return result.Errors;
            _sessionIds.Add(session.Id);
            session.SetSession();
            return Result.Success;
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

    public record DoctorSessionCreatedEvent(Session.Session session) : IDomainEvent;
    public record WorkingHours
    {
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        internal bool InDayLight => StartTime < EndTime; //To check if a doctor has midnight working hours (very rare)
    }
}
