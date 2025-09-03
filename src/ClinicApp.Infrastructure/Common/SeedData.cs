using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using System;

namespace ClinicApp.Infrastructure.Common;

public static class SeedData
{
    // Rooms
    public static readonly Guid Room1Id = new("a8a3e3e8-3d6c-4f4a-9a3d-5e6e1b2e0c1a");
    public static readonly Guid Room2Id = new("b8b4e4e9-4d7c-4f5a-9b4d-6e7e2c3e1d2b");

    // Users (correlating to an external identity system)
    public static readonly Guid Doctor1UserId = new("c8c5e5ea-5d8c-4f6a-9c5d-7e8e3d4e2e3c");
    public static readonly Guid Patient1UserId = new("d8d6e6eb-6d9c-4f7a-9d6d-8e9e4e5e3f4d");

    // Domain Entity IDs
    public static readonly Guid Doctor1Id = new("e8e7e7ec-7dac-4f8a-9e7d-9e0e5f6e4f5e");
    public static readonly Guid Patient1Id = new("f8f8e8ed-8dbc-4f9a-9f8d-0f1f6a7e5b6f");

    public static object[] Rooms =>
    new object[]
    {
        new { Id = Room1Id, Name = "General Examination Room" },
        new { Id = Room2Id, Name = "Cardiology Consultation Room" }
    };

    public static object[] Patients =>
    new object[]
    {
        new { Id = Patient1Id, UserId = Patient1UserId, FirstName = "Jane", LastName = "Smith" }
    };

    public static object[] Doctors =>
    new object[]
    {
        new
        {
            Id = Doctor1Id,
            UserId = Doctor1UserId,
            FirstName = "John",
            LastName = "Doe",
            RoomId = Room1Id,
            Major = "Cardiology"
        }
    };

    // Seeding for owned entities requires a bit more structure.
    // The anonymous type must match the property names in the owned entity.
    public static object[] DoctorWorkingTimes =>
    new object[]
    {
        new
        {
            DoctorId = Doctor1Id, // Foreign key to the Doctor
            WorkingDays = WorkingDays.Monday | WorkingDays.Tuesday | WorkingDays.Wednesday | WorkingDays.Thursday | WorkingDays.Friday,
            WorkingHours = new // The owned WorkingHours object
            {
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0),
                TimeZoneId = "UTC"
            }
        }
    };

    // Sessions
    public static readonly Guid Session1Id = new("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
    public static readonly Guid Session2Id = new("b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e");

    public static object[] Sessions =>
    new object[]
    {
        new
        {
            Id = Session1Id,
            DoctorId = Doctor1Id,
            PatientId = Patient1Id,
            RoomId = Room1Id,
            SessionStatus = SessionStatus.Set,
            CreatedAt = new DateTimeOffset(2025, 10, 10, 12, 0, 0, TimeSpan.Zero)
        },
        new
        {
            Id = Session2Id,
            DoctorId = Doctor1Id,
            PatientId = Patient1Id,
            RoomId = Room1Id,
            SessionStatus = SessionStatus.Pending,
            CreatedAt = new DateTimeOffset(2025, 10, 11, 12, 0, 0, TimeSpan.Zero)
        }
    };

    public static object[] SessionDates =>
    new object[]
    {
        new
        {
            SessionId = Session1Id,
            StartTime = new DateTimeOffset(2025, 10, 20, 10, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2025, 10, 20, 10, 30, 0, TimeSpan.Zero)
        },
        new
        {
            SessionId = Session2Id,
            StartTime = new DateTimeOffset(2025, 10, 21, 14, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2025, 10, 21, 15, 0, 0, TimeSpan.Zero)
        }
    };

    public static object[] SessionDescriptions =>
    new object[]
    {
        new { SessionId = Session1Id, content = "Follow-up consultation" },
        new { SessionId = Session2Id, content = "Initial diagnosis" }
    };
}
}