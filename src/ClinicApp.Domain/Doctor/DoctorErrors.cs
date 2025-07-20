using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Doctor
{
    public static class DoctorErrors
    {
        public const string SessionValidationError = "Doctor.SessionOverlapping";
        public static Error DoctorModifyValidationError = Error.Validation(
            code: "Doctor.Validation",
            description : "Can't modify properites of another doctor"
            );

        public static Error SessionOutOfWorkingDay(DayOfWeek sessionDay, WorkingDays doctorDays) =>
       Error.Conflict(
           code: "Doctor.WorkingDayConflict",
           description: "Session is out of the working days of the doctor.",
           metadata: new Dictionary<string, object>
           {
                { "SessionDay", sessionDay.ToString() },
                { "DoctorDays", GetDaysList(doctorDays) }
           });

        public static Error SessionOutOfWorkingHours(TimeRange sessionTime, WorkingHours workingHours) =>
            Error.Conflict(
                code: "Doctor.WorkingHoursConflict",
                description: "Session is out of the working hours of the doctor.",
                metadata: new Dictionary<string, object>
                {
                { "SessionTime", sessionTime.ToString() },
                { "DoctorWorkingHours", workingHours}
                });

        private static List<string> GetDaysList(WorkingDays daysEnum) =>
       Enum.GetValues<WorkingDays>()
           .Where(day => day != WorkingDays.None && daysEnum.HasFlag(day))
           .Select(day => day.ToString())
           .ToList();
    }
}
