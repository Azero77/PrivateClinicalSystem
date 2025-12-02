using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services.Sessions;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.DoctorAgg;

public record WorkingTime
{
    public WorkingDays WorkingDays { get; private set; }
    public WorkingHours WorkingHours { get; private set; } = null!;

    private List<TimeOff> _timesOff  = new();
    public IReadOnlyList<TimeOff> TimesOff => _timesOff;
    private WorkingTime() { }
    public static ErrorOr<WorkingTime> Create(TimeOnly startTime, TimeOnly endTime,
        WorkingDays workingDays, string? timeZoneId = null)
    {
        if (startTime > endTime)
        {
            return ScheduleErrors.TimeValidationError;
        }
        var workingHours = WorkingHours.Create(startTime, endTime, timeZoneId);
        if (workingHours.IsError)
            return workingHours.Errors;
        return new WorkingTime()
        {
            WorkingHours = workingHours.Value,
            WorkingDays = workingDays
        };
    }
    internal void AddTimeOff(TimeOff timeOff)
    {
        _timesOff.Add(timeOff);
    }

    internal ErrorOr<Success> CanAddBasedToSchedule(TimeRange sessionDate)
    {
        TimeZoneInfo doctorTimeZone;
        try
        {
            doctorTimeZone = TimeZoneInfo.FindSystemTimeZoneById(WorkingHours.TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            doctorTimeZone = TimeZoneInfo.Local;
        }
        var sessionStartInDoctorTz = TimeZoneInfo.ConvertTime(sessionDate.StartTime, doctorTimeZone);

        WorkingDays sessionDay = (WorkingDays)(1 << ((int)sessionStartInDoctorTz.DayOfWeek));
        var errors = new List<Error>();
        if ((sessionDay & WorkingDays) != sessionDay)
        {
            errors.Add(DoctorErrors.SessionOutOfWorkingDay(sessionStartInDoctorTz.DayOfWeek, this.WorkingDays));
        }
        else
        {
            var sessionEndInDoctorTz = TimeZoneInfo.ConvertTime(sessionDate.EndTime, doctorTimeZone);
            var sessionStartTimeOnly = TimeOnly.FromDateTime(sessionStartInDoctorTz.DateTime);
            var sessionEndTimeOnly = TimeOnly.FromDateTime(sessionEndInDoctorTz.DateTime);

            bool isWithinHours;
            if (WorkingHours.IsSameDayShift)
            {
                isWithinHours = sessionStartTimeOnly >= WorkingHours.StartTime &&
                                sessionEndTimeOnly <= WorkingHours.EndTime;
            }
            else // Overnight shift
            {
                if (sessionStartTimeOnly <= sessionEndTimeOnly) // Session doesn't cross midnight
                {
                    isWithinHours = (sessionStartTimeOnly >= WorkingHours.StartTime && sessionEndTimeOnly <= TimeOnly.MaxValue) || 
                                    (sessionStartTimeOnly >= TimeOnly.MinValue && sessionEndTimeOnly <= WorkingHours.EndTime);
                }
                else // Session crosses midnight
                {
                    isWithinHours = sessionStartTimeOnly >= WorkingHours.StartTime && sessionEndTimeOnly <= WorkingHours.EndTime;
                }
            }

            if (!isWithinHours)
            {
                errors.Add(DoctorErrors.SessionOutOfWorkingHours(sessionDate, this.WorkingHours));
            }
        }

        foreach (var timeoff in TimesOff)
        {
            if (TimeRange.IsOverlapping(TimeRange.Create(timeoff.StartDate, timeoff.EndDate).Value,
                    sessionDate))
            {
                errors.Add(DoctorErrors.SessionConflictsWithDoctorTimeOff(sessionDate, timeoff));
                break;
            }
        }
        return errors.Any() ? errors : Result.Success;
    }
}


public record TimeOff(DateTimeOffset StartDate,DateTimeOffset EndDate,string? reason);
