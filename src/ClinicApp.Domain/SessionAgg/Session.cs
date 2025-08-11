using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System.Data;

namespace ClinicApp.Domain.SessionAgg
{
    public class Session : AggregateRoot
    {
        private IClock _clock = null!;
        public static ErrorOr<Session> Create(Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       IClock clock,
                       SessionStatus session = SessionStatus.Pending
            )
        {
            if (sessionDate.StartTime.Date < clock.UtcNow)
            {
                return SessionErrors.SessionTimeInThePast.error;
            }

            
            var result = new Session()
            {
                Id = id,
                SessionDate = sessionDate,
                SessionDescription = sessionDescription,
                RoomId = roomId,
                SessionStatus = session,
                PatientId = patientId,
                DoctorId = doctorId,
                CreatedAt = clock.UtcNow,
                _clock = clock
            };
            result.PushChanges(SessionState.CreateSessionState(result.SessionStatus));
            return result;
        }

        public static ErrorOr<Session> Schedule(
                        Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       IClock clock,
                       UserRole role)
        {
            SessionStatus state;
            switch (role)
            {
                case UserRole.Doctor:
                    state = SessionStatus.Set;
                    break;
                case UserRole.Admin:
                    state = SessionStatus.Set;
                    break;
                case UserRole.Secretary:
                    state = SessionStatus.Pending;
                    break;
                default:
                    return SessionErrors.CantCreateSessionWithUserRole;
            }
            return Create(id,sessionDate,sessionDescription,roomId,patientId,doctorId,clock,state);
        }

        private Session() { } 
        public TimeRange SessionDate { get; private set; } = null!;
        public SessionDescription SessionDescription { get; private set; } = null!;
        public Guid RoomId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public SessionStatus SessionStatus { get; private set; }
        public SessionHistory SessionHistory { get; private set; } = new();
        public DateTime CreatedAt { get; private set; }

        internal bool IsDeleted => (SessionStatus & SessionStatus.Deleted) == SessionStatus.Deleted;
        internal bool IsFinished => (SessionStatus & SessionStatus.Finished) == SessionStatus.Finished;
        internal bool IsPaid => (SessionStatus & SessionStatus.Paid) == SessionStatus.Paid;
        public ErrorOr<Success> SetSession() //From Pending To Set
        {
            AddStatus(SessionStatus.Set);
            RemoveStatus(SessionStatus.Pending);
            PushChanges(SessionState.SetSessionState(_clock.UtcNow));
            return Result.Success;
        }

        public ErrorOr<Deleted> DeleteSession()
        {
            if (HasStatus(SessionStatus.Deleted))
                return SessionErrors.CantDeleteADeletedSession;
            AddStatus(SessionStatus.Deleted);
            PushChanges(SessionState.DeletedSessionState(_clock.UtcNow));
            return Result.Deleted;
        }

        public ErrorOr<Success> StartSession()
        {
            if(HasStatus(SessionStatus.Deleted))
                return SessionErrors.CantStartADeletedSession;
            AddStatus(SessionStatus.Started);
            PushChanges(SessionState.StartedSessionState(_clock.UtcNow));
            return Result.Success;
        }

        public ErrorOr<Success> FinishSession()
        {
            if(HasStatus(SessionStatus.Deleted))
                return SessionErrors.CantFinishADeletedSession;
           /* if (_clock.UtcNow.Day < SessionDate.StartTime.Day)
                return Error.Validation(code: "Session.Validation",
                    description: "Can't Finish a session in the future");*/
            AddStatus(SessionStatus.Finished);
            PushChanges(SessionState.FinishedSessionState(_clock.UtcNow));
            return Result.Success;
        }

        public ErrorOr<Success> RejectSession()
        {
            if (HasStatus(SessionStatus.Deleted) || HasStatus(SessionStatus.Finished))
                return SessionErrors.CantRejectADeletedOrFinishedSession;
            else if (_clock.UtcNow > SessionDate.StartTime)
                return SessionErrors.CantRejectASessionInTheFuture;
            AddStatus(SessionStatus.Pending);
            RemoveStatus(SessionStatus.Pending);
            PushChanges(SessionState.RejectedSessionState(_clock.UtcNow));
            return Result.Success;
        }

        public ErrorOr<Success> UpdateDate(TimeRange newTimeRange)
        {
            if (HasStatus(SessionStatus.Deleted))
            {
                return SessionErrors.CantUpdateDeletedSessions;
            }

            if (HasStatus(SessionStatus.Finished))
            {
                return SessionErrors.CantUpdateFinishedSessions;
            }
            AddStatus(SessionStatus.Updated);
            PushChanges(SessionState.UpdatedSessionState(this.SessionDate, newTimeRange, _clock.UtcNow));
            this.SessionDate = newTimeRange;
            return Result.Success;
        }


        //For dealing with domain events and session history at the same time
        private void PushChanges(SessionState state)
        {
            _domainEvents.Add(SessionDomainEventFactory.From(state));
            SessionHistory.AddNewState(state);
        }

        private void AddStatus(SessionStatus status)
        {
            SessionStatus |= status;
        }

        private void RemoveStatus(SessionStatus status)
        {
            SessionStatus &= ~status;
        }

        public bool HasStatus(SessionStatus status)
        {
            return (SessionStatus & status) == status;
        }
    }

    public record SessionDescription(string? content);
}
