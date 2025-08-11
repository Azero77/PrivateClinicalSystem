using Xunit;
using FluentAssertions;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class SessionTests
    {
        private IClock clock;
        public SessionTests()
        {
            clock = new FakerClock() { UtcNow = new DateTime(2025, 11, 2) };
        }
        private ErrorOr<Session> GetSession(TimeRange sessionTime) => Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), clock);
        [Fact]
        public void Create_Should_ReturnError_When_SessionIsInThePast()
        {
            // Arrange
            var sessionTime = TimeRange.Create(clock.UtcNow.AddDays(-1), clock.UtcNow.AddDays(-1).AddHours(1)).Value;

            // Act
            var result = GetSession(sessionTime);
            
            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be(SessionErrors.SessionTimeInThePast.code);
        }

        [Fact]
        public void Session_Should_TransitionThroughLifecycleCorrectly()
        {
            // Arrange
            var sessionTime = TimeRange.Create(clock.UtcNow.AddDays(1), clock.UtcNow.AddDays(1).AddHours(1)).Value;
            var session = GetSession(sessionTime).Value;

            // Act & Assert
            session.SessionStatus.Should().Be(SessionStatus.Pending);

            session.SetSession();
            session.SessionStatus.Should().Be(SessionStatus.Set);

            session.StartSession();
            session.SessionStatus.Should().Be(SessionStatus.Set | SessionStatus.Started);

            session.FinishSession();
            session.SessionStatus.Should().Be(SessionStatus.Set | SessionStatus.Started | SessionStatus.Finished); //because the date is in the future
        }

        [Fact]
        public void UpdateDate_Should_ReturnError_When_SessionIsFinished()
        {
            // Arrange
            var sessionTime = TimeRange.Create(clock.UtcNow.AddDays(1), clock.UtcNow.AddDays(1).AddHours(1)).Value;
            var session = GetSession(sessionTime).Value;
            session.SetSession();
            session.StartSession();
            session.FinishSession();

            // Act
            var newTime = TimeRange.Create(clock.UtcNow.AddDays(2), clock.UtcNow.AddDays(2).AddHours(1)).Value;
            var result = session.UpdateDate(newTime);

            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}
