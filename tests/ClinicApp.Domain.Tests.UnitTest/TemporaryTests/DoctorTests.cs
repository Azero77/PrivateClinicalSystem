using ClinicApp.Domain.SessionAgg;
using ClinicApp.Domain.Common.ValueObjects;
using FluentAssertions;
using ClinicApp.Domain.Common;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class DoctorTests
    {
        [Fact]
        public void AddSession_Should_ReturnError_When_SessionIsOutsideOfWorkingDays()
        {
            // Arrange
            var doctor = Factories.DoctorFactory;
            var fakerClock = new FakerClock { UtcNow = new DateTime(2025,7,21) };
            var sessionTime = TimeRange.Create(new DateTimeOffset(new DateTime(2025, 7, 22),TimeSpan.FromHours(0)),new DateTimeOffset(new DateTime(2025, 7, 22, 10, 30, 0),TimeSpan.FromHours(0))).Value; // Tuesday
            var session = Session.Schedule(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id,fakerClock,UserRole.Admin).Value;

            // Act
            var result = doctor.CanAddBasedToSchedule(session.SessionDate);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingDayConflict");
        }

        [Fact]
        public void AddSession_Should_ReturnError_When_SessionIsOutsideOfWorkingHours()
        {
            // Arrange
            var doctor = Factories.DoctorFactory;

            var fakerClock = new FakerClock { UtcNow = new DateTimeOffset(2025, 7, 21,0,0,0,TimeSpan.Zero) };
            var sessionTime = TimeRange.Create(new DateTimeOffset(2025, 7, 21, 8, 0, 0,TimeSpan.Zero), new DateTimeOffset(2025, 7, 21, 9, 0, 0,TimeSpan.Zero)).Value; // Monday, but too early
            var session = Session.Schedule(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id,fakerClock,UserRole.Admin).Value;

            // Act
            var result = doctor.CanAddBasedToSchedule(session.SessionDate);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingHoursConflict");
        }
    }
}
