using Moq;
using Xunit;
using FluentAssertions;
using ClinicApp.Domain.Doctor;
using ClinicApp.Domain.Session;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services.Sessions;
using ErrorOr;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class DoctorSessionSchedulerTests
    {
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly DoctorSessionScheduler _scheduler;

        public DoctorSessionSchedulerTests()
        {
            _sessionRepoMock = new Mock<ISessionRepository>();
            _scheduler = new DoctorSessionScheduler(_sessionRepoMock.Object);
        }

        [Fact]
        public async Task CreateSession_Should_ReturnError_WhenSessionIsOutsideOfWorkingDays()
        {
            // Arrange
            var doctor = Factories.DoctorFactory();
            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 22), new DateTime(2025, 7, 22, 10, 30, 0)).Value; // Tuesday
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            _sessionRepoMock.Setup(repo => repo.GetAllSessionsForDoctor(doctor)).ReturnsAsync(new List<Session.Session>());

            // Act
            var result = await _scheduler.CreateSession(session, doctor);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Doctor.WorkingDayConflict");
        }

        [Fact]
        public async Task CreateSession_Should_ReturnError_WhenSessionsOverlap()
        {
            // Arrange
            var doctor = Factories.DoctorFactory();


            var existingSessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 10, 0, 0), new DateTime(2025, 7, 21, 11, 0, 0)).Value;
            var existingSession = Session.Session.Create(Guid.NewGuid(), existingSessionTime, new SessionDescription("Existing"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            var newSessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 10, 30, 0), new DateTime(2025, 7, 21, 11, 30, 0)).Value;
            var newSession = Session.Session.Create(Guid.NewGuid(), newSessionTime, new SessionDescription("New"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            _sessionRepoMock.Setup(repo => repo.GetAllSessionsForDoctor(doctor)).ReturnsAsync(new List<Session.Session>());
            _sessionRepoMock.Setup(repo => repo.GetFutureSessionsDoctor(doctor)).ReturnsAsync(new List<Session.Session> { existingSession });

            // Act
            var result = await _scheduler.CreateSession(newSession, doctor);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Schedule.Conflict");
        }

        [Fact]
        public async Task CreateSession_Should_Succeed_WhenSlotIsAvailable()
        {
            // Arrange
            var doctor = Factories.DoctorFactory();

            var sessionTime = TimeRange.Create(new DateTime(2025, 7, 21, 14, 0, 0), new DateTime(2025, 7, 21, 15, 0, 0)).Value;
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), doctor.Id).Value;

            _sessionRepoMock.Setup(repo => repo.GetAllSessionsForDoctor(doctor)).ReturnsAsync(new List<Session.Session>());
            _sessionRepoMock.Setup(repo => repo.GetFutureSessionsDoctor(doctor)).ReturnsAsync(new List<Session.Session>());

            // Act
            var result = await _scheduler.CreateSession(session, doctor);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Created);
            session.HasStatus(SessionStatus.Set).Should().BeTrue();
        }
    }
}
