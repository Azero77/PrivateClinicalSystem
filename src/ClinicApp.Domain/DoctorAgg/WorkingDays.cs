namespace ClinicApp.Domain.DoctorAgg;

[Flags]
public enum WorkingDays : byte
{
    None = 0,
    Sunday = 1 << 0,
    Monday = 1 << 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6
}



public static class WorkingDaysExtensions
{
    public static List<bool> ToList(this WorkingDays workingDays)
    {
        return new List<bool>
        {
            workingDays.HasFlag(WorkingDays.Sunday),
            workingDays.HasFlag(WorkingDays.Monday),
            workingDays.HasFlag(WorkingDays.Tuesday),
            workingDays.HasFlag(WorkingDays.Wednesday),
            workingDays.HasFlag(WorkingDays.Thursday),
            workingDays.HasFlag(WorkingDays.Friday),
            workingDays.HasFlag(WorkingDays.Saturday)
        };
    }

    public static List<string> ToListDays(this WorkingDays workingDays)
    {
        var result = new List<string>();
        var values = Enum.GetValues<WorkingDays>();

        foreach (var value in values)
        {
            if (workingDays.HasFlag(value))
                result.Add(value.ToString());
        }

        return result;
    }

}