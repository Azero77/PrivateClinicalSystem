using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Infrastructure.Converters;

public class SessionConverter : IConverter<Session, SessionDataModel>
{
    public Session MapToEntity(SessionDataModel model)
    {
        var session = Session.Create(
            model.Id,
            TimeRange.Create(model.StartTime, model.EndTime).Value,
            new SessionDescription(model.Content),
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
            StartTime = entity.SessionDate.StartTime,
            EndTime = entity.SessionDate.EndTime,
            Content = entity.SessionDescription.content,
            RoomId = entity.RoomId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            SessionStatus = entity.SessionStatus,
            CreatedAt = entity.CreatedAt
        };
    }
}
