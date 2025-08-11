namespace ClinicApp.Domain.DoctorAgg
{
    public record WorkingHours
    {
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        internal bool InDayLight => StartTime < EndTime; //To check if a doctor has midnight working hours (very rare)
        private WorkingHours(TimeOnly startTime, TimeOnly endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
        public static WorkingHours Create(TimeOnly startTime, TimeOnly endTime)
        {
            return new WorkingHours(startTime,endTime);
        }
    }
}
