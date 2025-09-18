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
        public Doctor(Guid id, 
            Guid userId,
            string firstName,
            string lastName,
            Guid roomId,
            WorkingDays workingDays, 
            WorkingHours workingHours,
            string? major = null
            ) : base(id,userId,firstName,lastName)
        {
            WorkingTime = WorkingTime.Create(workingHours.StartTime, workingHours.EndTime,
                workingDays, workingHours.TimeZoneId).Value;
            RoomId = roomId;
            Major = major;
        }
        private Doctor()
        {

        }
        public WorkingTime WorkingTime { get; private set; } = null!;
        public string? Major { get; private set; } = null!;
        public Guid RoomId { get; private set; }
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

        public void UpdateMajor(string? major)
        {
            Major = major;
        }

        public void UpdateRoom(Guid roomId)
        {
            RoomId = roomId;
        }

        public void UpdateWorkingTime(WorkingTime workingTime)
        {
            WorkingTime = workingTime;
        }
    }
}
