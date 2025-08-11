using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Common.ValueObjects
{
    public record TimeRange
    {
        private TimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public static ErrorOr<TimeRange> Create(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
                return SessionErrors.SessionTimeValidationError.error;
            return new TimeRange(startTime, endTime);
        }
        public TimeSpan Duration => EndTime - StartTime;

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        public static bool IsOverlapping(TimeRange t1, TimeRange t2)
        {
            return t1.StartTime < t2.EndTime
                    &&
                   t2.StartTime < t1.EndTime;
        }

        public bool IsMidnight => StartTime.Day != EndTime.Day;
    }
}
