
using ClinicApp.Domain.Doctor;
using ClinicApp.Domain.Session;
using ClinicApp.Domain.Common.ValueObjects;
using Xunit;
using FluentAssertions;
using ErrorOr;
using Moq;
using ClinicApp.Domain.Services.Sessions;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class DoctorTests
    {
        [Fact]
        public void AddSession_Should_ReturnError_When_SessionIsOutsideOfWorkingDays()
        {
            // Arrange
            var doctor = new Doctor.Doctor(Guid.NewGuid(),
                WorkingDays.Monday | WorkingDays.Wednesday,
                WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)));
            
            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 22), new DateTime(2025, 7, 22, 10, 30, 0)).Value; // Tuesday
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            // Act
            var result = doctor.AddSession(session.SessionDate);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingDayConflict");
        }

        [Fact]
        public void AddSession_Should_ReturnError_When_SessionIsOutsideOfWorkingHours()
        {
            // Arrange
            var doctor = new Doctor.Doctor(Guid.NewGuid(),
                WorkingDays.Monday,
                WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)));
            
            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 8, 0, 0), new DateTime(2025, 7, 21, 9, 0, 0)).Value; // Monday, but too early
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            // Act
            var result = doctor.AddSession(session.SessionDate);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingHoursConflict");
        }
    }
}
