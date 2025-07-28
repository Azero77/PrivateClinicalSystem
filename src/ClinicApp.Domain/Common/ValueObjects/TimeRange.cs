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
    }
}
