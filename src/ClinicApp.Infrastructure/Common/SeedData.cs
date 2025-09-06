using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.AspNetCore.Http;
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

        public static IReadOnlyList<Room> Rooms => new[]
        {
            new Room ( Room1Id,"Room A" ),
            new Room (Room2Id,"Room B" )
        };

        public static IReadOnlyList<Doctor> Doctors => new[]
        {
            new Doctor
            (
                Doctor1Id,
                Guid.Parse("11111111-2222-2222-2222-111111111111"),
                "Alice",
                "Smith",
                Room1Id,
                (WorkingDays) 62,
                WorkingHours.Create(new TimeOnly(9, 0),endTime: new TimeOnly(17, 0),"UTC").Value,
                "Cardiology"
            ),
            new Doctor
            (
                Doctor2Id,
                Guid.Parse("11111111-3333-3333-3333-111111111111"), // UserId for Doctor2
                "Bob",
                "Johnson",
                Room2Id,
                (WorkingDays)124, // Tue–Sat bitmask
                WorkingHours.Create(new TimeOnly(10, 0), new TimeOnly(18, 0), "UTC").Value,
                "Dermatology"
            )

        };

        public static IReadOnlyList<Patient> Patients => new[]
        {
            new Patient(
                Patient1Id,
                Guid.Parse("55555555-aaaa-aaaa-aaaa-555555555555"), // deterministic UserId
                "Charlie",
                "Brown"
            ),
            new Patient(
                Patient2Id,
                Guid.Parse("66666666-bbbb-bbbb-bbbb-666666666666"), // deterministic UserId
                "Diana",
                "Prince"
            )
        };

        public static IReadOnlyList<Session> Sessions(IClock? clock = null)
        {
            if (clock is null)
                clock = new Clock();

            var day = clock.UtcNow.AddDays(1).Date;
            return new[]
            {
               Session.Create(
                    id: Session1Id,
                    sessionDate: TimeRange.Create(
                        startTime: new DateTimeOffset(DateOnly.FromDateTime(day),new TimeOnly(10,30),TimeSpan.FromHours(0)),
                        endTime: new DateTimeOffset(DateOnly.FromDateTime(day),new TimeOnly(11,30),TimeSpan.FromHours(0))
                    ).Value,
                    sessionDescription: new SessionDescription("Initial Consultation"),
                    roomId: Room1Id,
                    patientId: Patient1Id,
                    doctorId: Doctor1Id,
                    clock: new SystemClock(),
                    session: SessionStatus.Set
                ).Value
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
