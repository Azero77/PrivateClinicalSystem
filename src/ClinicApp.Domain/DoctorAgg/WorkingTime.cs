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
        WorkingDays workingDays)
    {
        if (startTime > endTime)
        {
            return ScheduleErrors.TimeValidationError;
        }
        return new WorkingTime()
        {
            WorkingHours = WorkingHours.Create(startTime, endTime),
            WorkingDays = workingDays
        };
    }
    internal void AddTimeOff(TimeOff timeOff)
    {
        _timesOff.Add(timeOff);
    }

    internal ErrorOr<Success> CanAddBasedToSchedule(TimeRange sessionDate)
    {
        WorkingDays sessionDay = (WorkingDays)(1 << ((int)sessionDate.StartTime.DayOfWeek));
        var errors = new List<Error>();
        if ((sessionDay & WorkingDays) != sessionDay)
        {
            errors.Add(DoctorErrors.SessionOutOfWorkingDay(sessionDate.StartTime.DayOfWeek, this.WorkingDays));
        }
        //write here
        else if (!(TimeOnly.FromDateTime(sessionDate.StartTime) > WorkingHours.StartTime &&
               TimeOnly.FromDateTime(sessionDate.EndTime) < WorkingHours.EndTime))
        {
            errors.Add(DoctorErrors.SessionOutOfWorkingHours(sessionDate, this.WorkingHours));
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


public record TimeOff(DateTime StartDate,DateTime EndDate,string? reason);
