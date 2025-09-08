using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Infrastructure.Converters;

public class SessionConverter : IConverter<Session, SessionDataModel>
{
    public Session MapToEntity(SessionDataModel model)
    {
        var session = Session.Create(
            model.Id,
            model.SessionDate,
            model.SessionDescription,
            model.RoomId,
            model.PatientId,
            model.DoctorId,
            new SystemClock(), // Assuming a clock is needed
            model.SessionStatus
        ).Value;

        return session;
    }

    public SessionDataModel MapToData(Session entity)
    {
        return new SessionDataModel
        {
            Id = entity.Id,
            SessionDate = entity.SessionDate,
            SessionDescription = entity.SessionDescription,
            RoomId = entity.RoomId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            SessionStatus = entity.SessionStatus,
            SessionHistory = entity.SessionHistory,
            CreatedAt = entity.CreatedAt
        };
    }
}
