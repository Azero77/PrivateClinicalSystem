using ErrorOr;

namespace ClinicApp.Domain.Session
{
    public record SessionDate
    {
        private SessionDate(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public static ErrorOr<SessionDate> Create(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
                return SessionErrors.SessionTimeValidationError.error;
            return new SessionDate(startTime, endTime);
        }
        public TimeSpan Duration => EndTime - StartTime;

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
    }
}
