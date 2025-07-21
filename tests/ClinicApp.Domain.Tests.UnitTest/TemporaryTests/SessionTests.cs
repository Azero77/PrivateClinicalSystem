
using ClinicApp.Domain.Session;
using ClinicApp.Domain.Common.ValueObjects;
using Xunit;
using FluentAssertions;

namespace ClinicApp.Domain.Tests.UnitTest.TemporaryTests
{
    public class SessionTests
    {
        [Fact]
        public void Create_Should_ReturnError_When_SessionIsInThePast()
        {
            // Arrange
            var sessionTime = TimeRange.Create(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-1).AddHours(1)).Value;

            // Act
            var result = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be(SessionErrors.SessionTimeInThePast.code);
        }

        [Fact]
        public void Session_Should_TransitionThroughLifecycleCorrectly()
        {
            // Arrange
            var sessionTime = TimeRange.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)).Value;
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;

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
            var sessionTime = TimeRange.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)).Value;
            var session = Session.Session.Create(Guid.NewGuid(), sessionTime, new SessionDescription("Test"), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
            session.SetSession();
            session.StartSession();
            session.FinishSession();

            // Act
            var newTime = TimeRange.Create(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1)).Value;
            var result = session.UpdateDate(newTime);

            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}
