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
        public Doctor(Guid id, WorkingDays workingDays, WorkingHours workingHours) : base(id)
        {
            WorkingDays = workingDays;
            WorkingHours = workingHours;
        }
        public WorkingDays WorkingDays { get; private set; }
        public WorkingHours WorkingHours { get; private set; } = null!;
        public ErrorOr<Success> AddSession(TimeRange sessionDate)
        {
            var canAdd = SessionConflictsWithDoctor(sessionDate);
            if (canAdd.IsError)
            {
                return canAdd.Errors;
            }
            return Result.Success;
        }

        internal ErrorOr<Success> SessionConflictsWithDoctor(TimeRange sessionDate)
        {
            WorkingDays sessionDay = (WorkingDays)(1 << ((int)sessionDate.StartTime.DayOfWeek));

            if (SessionConflictsWithWorkingDays(sessionDay))
            {
                return DoctorErrors.SessionOutOfWorkingDay(sessionDate.StartTime.DayOfWeek, this.WorkingDays);
            }
            else if (SessionConflictsWithWorkingHours(sessionDate))
            {
                return DoctorErrors.SessionOutOfWorkingHours(sessionDate, this.WorkingHours);
            }
            return Result.Success;
        }

        private bool SessionConflictsWithWorkingDays(WorkingDays sessionDay)
        {
            return (sessionDay & WorkingDays) != sessionDay;
        }

        private bool SessionConflictsWithWorkingHours(TimeRange sessionDate)
        {
            return !(
                            TimeOnly.FromDateTime(sessionDate.StartTime) > WorkingHours.StartTime &&
                            TimeOnly.FromDateTime(sessionDate.EndTime) < WorkingHours.EndTime);
        }

    }

    public record WorkingHours
    {
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        internal bool InDayLight => StartTime < EndTime; //To check if a doctor has midnight working hours (very rare)

        public static WorkingHours Create(TimeOnly startTime, TimeOnly endTime)
        {
            return new WorkingHours()
            {
                StartTime = startTime,
                EndTime = endTime
            };
        }
    }
}
