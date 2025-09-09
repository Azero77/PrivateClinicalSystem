using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System.Data;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("ClinicApp.Domain.Tests.UnitTest")]


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
            result._domainEvents.Add(new SessionCreatedDomainEvent(result));
            return result;
        }

        
        internal static ErrorOr<Session> Schedule(
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
        public DateTimeOffset CreatedAt { get; private set; }

        public bool IsDeleted => (SessionStatus & SessionStatus.Deleted) == SessionStatus.Deleted;
        public bool IsFinished => (SessionStatus & SessionStatus.Finished) == SessionStatus.Finished;
        public bool IsPaid => (SessionStatus & SessionStatus.Paid) == SessionStatus.Paid;
        public ErrorOr<Success> SetSession() //From Pending To Set
        {
            AddStatus(SessionStatus.Set);
            RemoveStatus(SessionStatus.Pending);
            _domainEvents.Add(new SetSessionDomainEvent(Id,_clock.UtcNow));
            return Result.Success;
        }

        public ErrorOr<Deleted> DeleteSession()
        {
            if (HasStatus(SessionStatus.Deleted))
                return SessionErrors.CantDeleteADeletedSession;
            AddStatus(SessionStatus.Deleted);
            _domainEvents.Add(new DeletedSessionDomainEvent(Id,_clock.UtcNow));
            return Result.Deleted;
        }

        public ErrorOr<Success> StartSession()
        {
            if(HasStatus(SessionStatus.Deleted))
                return SessionErrors.CantStartADeletedSession;
            AddStatus(SessionStatus.Started);
            _domainEvents.Add(new StartedSessionDomainEvent(Id,_clock.UtcNow));
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
            _domainEvents.Add(new FinishedSessionDomainEvent(Id,_clock.UtcNow));
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
            _domainEvents.Add(new RejectedSessionDomainEvent(Id,_clock.UtcNow));
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
            this.SessionDate = newTimeRange;
            _domainEvents.Add(new UpdatedSessionDomainEvent(Id,this.SessionDate, newTimeRange, _clock.UtcNow));
            return Result.Success;
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

