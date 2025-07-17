using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Session
{
    public class Session : Entity
    {
        public Session(Guid id,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       SessionStatus session,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId) : base(id)
        {
            SessionDate = sessionDate;
            SessionDescription = sessionDescription;
            RoomId = roomId;
            SessionStatus = session;
            PatientId = patientId;
            DoctorId = doctorId;
        }
        public TimeRange SessionDate { get; private set; }
        public SessionDescription SessionDescription { get; private set; }
        public Guid RoomId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public SessionStatus SessionStatus { get; private set; }
    }

    public record SessionDescription(object content);
}
