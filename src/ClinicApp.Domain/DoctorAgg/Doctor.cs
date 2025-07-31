using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.DoctorAgg
{
    public class Doctor : Member
    {
        public Doctor(Guid id, WorkingDays workingDays, WorkingHours workingHours) : base(id)
        {
            WorkingTime = WorkingTime.Create(workingHours.StartTime, workingHours.EndTime,
                workingDays).Value;
        }
        private Doctor()
        {

        }
        public WorkingTime WorkingTime { get; private set; } = null!;
        public string? Major { get; private set; } = null!;
        public ErrorOr<Success> CanAddBasedToSchedule(TimeRange sessionDate)
        {
            var result = WorkingTime.CanAddBasedToSchedule(sessionDate);
            if (result.IsError)
                return result.Errors;
            return Result.Success;
        }
        public void AddTimeOff(TimeOff newtimeOff)
        {
            foreach (var timeoff in WorkingTime.TimesOff)
            {

            }
        }
    }
}
