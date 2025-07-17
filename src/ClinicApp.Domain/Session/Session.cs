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
                       SessionStatus session,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId)
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
        public DateTime LastUpdatedAt { get; private set; }
        internal ErrorOr<Success> SetSession() //From Pending To Set
        {
            ErrorOr<Success> result = ChangeState(SessionStatus.Pending, SessionStatus.Set);
            return result.IsError ? Error.Failure(SessionErrors.SessionModifyState,"Can't Set without Pending status") : result;
        }

        internal ErrorOr<Success> DeleteSession() //From Pending To Set
        {
            ErrorOr<Success> result = ChangeState(SessionStatus.Deleted);
            return result.IsError ? Error.Failure(SessionErrors.SessionModifyState, "Can't Set without Pending status") : result;
        }

        internal ErrorOr<Success> ChangeState(SessionStatus prev, SessionStatus next)
        {
            if ((this.SessionStatus & prev) == prev)
            {
                this.SessionStatus &= ~prev;
                this.SessionStatus |= next;
                return Result.Success;
            }
            return Error.Failure();
        }
        internal ErrorOr<Success> ChangeState(SessionStatus next)
        {
            this.SessionStatus |= next;
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
            this.SessionDate = newTimeRange;
            _domainEvents.Add(new SessionUpdated(newTimeRange));
            return Result.Success;
        }
    }

    public record SessionDescription(object content);
    public record SessionUpdated(TimeRange newTimeRange) : IDomainEvent;
}
