using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ClinicApp.Domain.Session
{
    public class Session : AggregateRoot
    {
        public static ErrorOr<Session> Create(Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       SessionStatus session = SessionStatus.Pending)
        {
            if (sessionDate.StartTime.Date < DateTime.UtcNow.Date)
            {
                return SessionErrors.SessionTimeInThePast.error;
            }
            return new Session()
            {
                SessionDate = sessionDate,
                SessionDescription = sessionDescription,
                RoomId = roomId,
                SessionStatus = session,
                PatientId = patientId,
                DoctorId = doctorId
            };
        }
        private Session() { } 
        public TimeRange SessionDate { get; private set; } = null!;
        public SessionDescription SessionDescription { get; private set; } = null!;
        public Guid RoomId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public SessionStatus SessionStatus { get; private set; }
        public SessionHistory SessionHistory { get; private set; } = new();

        internal bool IsDeleted => (SessionStatus & SessionStatus.Deleted) == SessionStatus.Deleted;
        internal bool IsFinished => (SessionStatus & SessionStatus.Finished) == SessionStatus.Finished;
        internal bool IsPaid => (SessionStatus & SessionStatus.Paid) == SessionStatus.Paid;
        internal ErrorOr<Success> SetSession() //From Pending To Set
        {
            this.SessionStatus |= SessionStatus.Set;
            this.SessionStatus &= ~SessionStatus.Pending;
            SessionHistory.AddNewState(SessionState.SetSessionState(DateTime.UtcNow));
            
            return Result.Success;
        }

        internal ErrorOr<Deleted> DeleteSession()
        {
            if (IsDeleted)
                return Error.Validation(code: "Session.Validation",
                    description: "Can't Delete a deleted session");
            SessionStatus |= SessionStatus.Deleted;
            PushChanges(SessionState.DeletedSessionState(DateTime.UtcNow));
            return Result.Deleted;
        }

        internal ErrorOr<Success> StartSession()
        {
            if(IsDeleted)
                return Error.Validation(code: "Session.Validation",
                   description: "Can't Start a deleted session");
            SessionStatus |= SessionStatus.Started;
            PushChanges(SessionState.StartedSessionState(DateTime.UtcNow));
            return Result.Success;
        }

        internal ErrorOr<Success> FinishSession()
        {
            if(IsDeleted)
                return Error.Validation(code: "Session.Validation",
                   description: "Can't Finish a deleted session");
            if (DateTime.UtcNow < SessionDate.StartTime)
                return Error.Validation(code: "Session.Validation",
                    description: "Can't Finish a session in the future");
            SessionStatus |= SessionStatus.Finished;
            PushChanges(SessionState.FinishedSessionState(DateTime.UtcNow));
            return Result.Success;
        }

        internal ErrorOr<Success> RejectSession()
        {
            if (IsDeleted || IsFinished)
                return Error.Validation(code: "Session.Validation",
                   description: "Can't Reject a deleted or finished session");
            else if (DateTime.UtcNow > SessionDate.StartTime)
                return Error.Validation(code: "Session.Validation",
                    description: "Can't Reject a session in the future");
            SessionStatus |= SessionStatus.Rejected;
            SessionStatus &= ~SessionStatus.Pending;
            PushChanges(SessionState.RejectedSessionState(DateTime.UtcNow));
            return Result.Success;
        }

        internal ErrorOr<Success> UpdateDate(TimeRange newTimeRange)
        {
            if ((SessionStatus & SessionStatus.Deleted) == SessionStatus.Deleted)
            {
                return Error.Validation(SessionErrors.SessionDeletionError,
                    "Can't Update Deleted Sessions");
            }

            if ((SessionStatus & SessionStatus.Finished) == SessionStatus.Finished)
            {
                return Error.Validation(SessionErrors.SessionDeletionError,
                    "Can't Update Finished Sessions");
            }
            SessionStatus |= SessionStatus.Updated;
            PushChanges(SessionState.UpdatedSessionState(this.SessionDate, newTimeRange, DateTime.UtcNow));
            this.SessionDate = newTimeRange;
            return Result.Success;
        }

        private void PushChanges(SessionState state)
        {
            _domainEvents.Add(SessionDomainEventFactory.From(state));
            SessionHistory.AddNewState(state);
        }
    }

    public record SessionDescription(object content);
}
