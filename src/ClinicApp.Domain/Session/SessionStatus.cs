namespace ClinicApp.Domain.Session;

[Flags]
public enum SessionStatus
{
    Pending = 0,
    Set = 1,
    Updated = 2,
    Started = 4,
    Finished = 8,
    Deleted = 16,
    //Financial
    Paid = 32,
    Unpaid = 64
}
