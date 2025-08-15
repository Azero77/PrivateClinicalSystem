using Moq;
using Xunit;
using FluentAssertions;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services.Sessions;
using ErrorOr;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Domain.Common;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class DoctorSessionSchedulerTests
    {
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly DoctorScheduler _scheduler;

        public DoctorSessionSchedulerTests()
        {
            _sessionRepoMock = new Mock<ISessionRepository>();
            _scheduler = new DoctorScheduler(_sessionRepoMock.Object);
        }

        [Fact]
        public async Task CreateSession_Should_ReturnError_WhenSessionIsOutsideOfWorkingDays()
        {
            // Arrange
            var doctor = Factories.DoctorFactory;
            var fakerClock = new FakerClock { UtcNow =  new DateTime(2025,7,21)};
            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 22), new DateTime(2025, 7, 22, 10, 30, 0)).Value; // Tuesday
            var session = Session.Schedule(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id,fakerClock,UserRole.Admin).Value;

            _sessionRepoMock.Setup(repo => repo.GetAllSessionsForDoctor(doctor)).ReturnsAsync(new List<Session>());

            // Act
            var result = await _scheduler.CreateSession(session.Id, session.SessionDate, session.SessionDescription, session.RoomId, session.PatientId, session.DoctorId, fakerClock, UserRole.Admin, doctor);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingDayConflict");
        }

        [Fact]
        public async Task CreateSession_Should_ReturnError_WhenSessionsOverlap()
        {
            // Arrange
            var doctor = Factories.DoctorFactory;

            var fakerClock = new FakerClock { UtcNow = new DateTime(2025, 7, 21) };

            var existingSessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 10, 0, 0), new DateTime(2025, 7, 21, 11, 0, 0)).Value;
            var existingSession = Session.Schedule(Guid.NewGuid(), existingSessionTime, new SessionDescription("Existing"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id,fakerClock,UserRole.Admin).Value;

            var newSessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 10, 30, 0), new DateTime(2025, 7, 21, 11, 30, 0)).Value;
            var newSession = Session.Schedule(Guid.NewGuid(), newSessionTime, new SessionDescription("New"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id, fakerClock,UserRole.Admin).Value;

            _sessionRepoMock.Setup(repo => repo.GetSessionsForDoctorOnDay(doctor.Id,newSession.SessionDate.StartTime)).ReturnsAsync(new List<Session> { existingSession });
            _sessionRepoMock.Setup(repo => repo.GetSesssionsForDoctorOnDayAndAfter(doctor.Id,newSession.SessionDate.StartTime)).ReturnsAsync(new List<Session> { existingSession });

            // Act
            var result = await _scheduler.CreateSession(newSession.Id, newSession.SessionDate, newSession.SessionDescription, newSession.RoomId, newSession.PatientId, newSession.DoctorId, fakerClock, UserRole.Admin, doctor);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Schedule.Conflict");
        }

        [Fact]
        public async Task CreateSession_Should_Succeed_WhenSlotIsAvailable()
        {
            // Arrange
            var doctor = Factories.DoctorFactory;

            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 14, 0, 0), new DateTime(2025, 7, 21, 15, 0, 0)).Value;
            var fakerClock = new FakerClock { UtcNow = new DateTime(2025, 7, 21) };

            var session = Session.Schedule(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id, fakerClock,UserRole.Admin).Value;

            _sessionRepoMock.Setup(repo => repo.GetSessionsForDoctorOnDay(doctor.Id, session.SessionDate.StartTime)).ReturnsAsync(new List<Session>());
            _sessionRepoMock.Setup(repo => repo.GetSesssionsForDoctorOnDayAndAfter(doctor.Id, session.SessionDate.StartTime)).ReturnsAsync(new List<Session>());

            // Act
            var result = await _scheduler.CreateSession(session.Id, session.SessionDate, session.SessionDescription, session.RoomId, session.PatientId, session.DoctorId, fakerClock, UserRole.Admin, doctor);

            // Assert
            result.IsError.Should().BeFalse();
            session.HasStatus(SessionStatus.Set).Should().BeTrue();
        }
    }
}
