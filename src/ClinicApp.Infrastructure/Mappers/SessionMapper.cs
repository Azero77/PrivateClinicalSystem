using ClinicApp.Domain.Common;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using System;
using System.Reflection;

namespace ClinicApp.Infrastructure.Mappers;

public static class SessionMapper
{
    public static SessionDataModel ToDataModel(this Session session)
    {
        // NOTE: The SessionDataModel does not have an 'Id' property, so it is not mapped here.
        return new SessionDataModel
        {
            SessionDate = session.SessionDate,
            SessionDescription = session.SessionDescription,
            RoomId = session.RoomId,
            PatientId = session.PatientId,
            DoctorId = session.DoctorId,
            SessionStatus = session.SessionStatus,
            SessionHistory = session.SessionHistory,
            CreatedAt = session.CreatedAt
        };
    }

    public static Session ToDomain(this SessionDataModel sessionData)
    {
        var session = Session.Reconstitute(       
                 sessionData.Id,                
                 sessionData.SessionDate,       
                 sessionData.SessionDescription,
                 sessionData.RoomId,            
                 sessionData.PatientId,         
                 sessionData.DoctorId,          
                 sessionData.SessionStatus,     
                 sessionData.SessionHistory,    
                 sessionData.CreatedAt          
             );     
        return session;
    }
}
