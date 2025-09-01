using ErrorOr;

namespace ClinicApp.Domain.DoctorAgg
{
    public record WorkingHours
    {
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public string TimeZoneId { get; private set; }
        internal bool IsSameDayShift => StartTime < EndTime; //To check if a doctor has midnight working hours (very rare)
        private WorkingHours(TimeOnly startTime, TimeOnly endTime, string timeZoneId)
        {
            StartTime = startTime;
            EndTime = endTime;
            TimeZoneId = timeZoneId;
        }
        public static ErrorOr<WorkingHours> Create(TimeOnly startTime, TimeOnly endTime,string? timeZoneId = null)
        {
            if (timeZoneId is null)
                timeZoneId = TimeZoneInfo.Local.Id;
            else if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId,out TimeZoneInfo? nully))
            {
                return Error.Validation("TimeZones.NotFound", $"Timezone With {timeZoneId} is not Found");
            }

            return new WorkingHours(startTime, endTime, timeZoneId);
        }

        public TimeSpan GetOffsetFromUtc(DateOnly date)
        {
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var offset = timezoneInfo.GetUtcOffset(date.ToDateTime(StartTime,DateTimeKind.Unspecified));
            return offset;
        }

        /// <summary>
        /// Converts start time to UTC for a specific date.
        /// </summary>
        public DateTimeOffset ToUtcStart(DateOnly date)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var localDateTime = date.ToDateTime(StartTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);
        }

        /// <summary>
        /// Converts end time to UTC for a specific date.
        /// Handles shifts that span midnight.
        /// </summary>
        public DateTimeOffset ToUtcEnd(DateOnly date)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var endDate = date;
            if (!IsSameDayShift) // If shift crosses midnight
                endDate = date.AddDays(1);

            var localDateTime = endDate.ToDateTime(EndTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);
        }
    }
}
