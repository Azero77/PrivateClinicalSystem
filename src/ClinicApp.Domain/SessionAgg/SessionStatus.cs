namespace ClinicApp.Domain.SessionAgg;

[Flags]
public enum SessionStatus : byte
{
    Pending = 0,
    Set = 1,
    Rejected = 2,
    Updated = 4,
    Started = 8,
    Finished = 16,
    Deleted = 32,
    //Financial
    Paid = 64,
    Unpaid = 128
}
