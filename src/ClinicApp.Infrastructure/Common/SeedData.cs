using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using System;
using System.Collections.Generic;

namespace ClinicApp.Infrastructure.Persistance.Seeding
{
    public static class SeedData
    {
        // Predefined IDs (so relationships stay consistent)
        public static readonly Guid Room1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Room2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Doctor1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid Doctor2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid Patient1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        public static readonly Guid Patient2Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        public static readonly Guid Session1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        public static readonly Guid OutboxMsg1Id = Guid.Parse("88888888-8888-8888-8888-888888888888");

        public static IReadOnlyList<RoomDataModel> Rooms => new[]
        {
            new RoomDataModel { Id = Room1Id, Name = "Room A" },
            new RoomDataModel { Id = Room2Id, Name = "Room B" }
        };

        public static IReadOnlyList<DoctorDataModel> Doctors => new[]
        {
            new DoctorDataModel
            {
                Id = Doctor1Id,
                UserId = Guid.Parse("11111111-2222-2222-2222-111111111111"),
                FirstName = "Alice",
                LastName = "Smith",
                RoomId = Room1Id,
                WorkingTime = WorkingTime.Create(new TimeOnly(9, 0),new TimeOnly(17, 0),(WorkingDays) 62,"UTC").Value,
                Major = "Cardiology"
            },
            new DoctorDataModel
            {
                Id = Doctor2Id,
                UserId = Guid.Parse("11111111-3333-3333-3333-111111111111"),
                FirstName = "Bob",
                LastName = "Johnson",
                RoomId = Room2Id,
                WorkingTime = WorkingTime.Create(new TimeOnly(10, 0),new TimeOnly(18, 0),(WorkingDays) 124,"UTC").Value,
                Major = "Dermatology"
            }
        };

        public static IReadOnlyList<PatientDataModel> Patients => new[]
        {
            new PatientDataModel
            {
                Id = Patient1Id,
                UserId = Guid.Parse("55555555-aaaa-aaaa-aaaa-555555555555"),
                FirstName = "Charlie",
                LastName = "Brown"
            },
            new PatientDataModel
            {
                Id = Patient2Id,
                UserId = Guid.Parse("66666666-bbbb-bbbb-bbbb-666666666666"),
                FirstName = "Diana",
                LastName = "Prince"
            }
        };

        public static IReadOnlyList<SessionDataModel> Sessions(IClock? clock = null)
        {
            if (clock is null)
                clock = new Clock();

            var day = clock.UtcNow.AddDays(1).Date;
            return new[]
            {
                new SessionDataModel
                {
                    Id = Session1Id,
                    SessionDate = TimeRange.Create(
                        startTime: new DateTimeOffset(DateOnly.FromDateTime(day), new TimeOnly(10, 30), TimeSpan.FromHours(0)),
                        endTime: new DateTimeOffset(DateOnly.FromDateTime(day), new TimeOnly(11, 30), TimeSpan.FromHours(0))
                    ).Value,
                    SessionDescription = new SessionDescription("Initial Consultation"),
                    RoomId = Room1Id,
                    PatientId = Patient1Id,
                    DoctorId = Doctor1Id,
                    SessionStatus = SessionStatus.Set,
                    CreatedAt = clock.UtcNow
                }
            };
        }

        public static IReadOnlyList<OutBoxMessage> OutboxMessages => new[]
        {
            new OutBoxMessage
            (
                OutboxMsg1Id,
                "InitialMessage",
                "System initialized",
                DateTime.UtcNow
            )
        };
    }
}