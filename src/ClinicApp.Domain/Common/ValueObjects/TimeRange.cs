using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Common.ValueObjects
{
    public record TimeRange
    {
        private TimeRange(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public static ErrorOr<TimeRange> Create(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (startTime > endTime)
                return SessionErrors.SessionTimeValidationError.error;
            return new TimeRange(startTime, endTime);
        }
        public TimeSpan Duration => EndTime - StartTime;

        public DateTimeOffset StartTime { get; }
        public DateTimeOffset EndTime { get; }


        internal DateTimeOffset StartTimeUTC => StartTime.ToUniversalTime();
        internal DateTimeOffset EndTimeUTC => EndTime.ToUniversalTime();

        public static bool IsOverlapping(TimeRange t1, TimeRange t2)
        {
            return t1.StartTimeUTC < t2.EndTimeUTC
                    &&
                   t2.StartTimeUTC < t1.EndTimeUTC;
        }


        public bool IsMidnight => StartTime.Day != EndTime.Day;
    }
}
