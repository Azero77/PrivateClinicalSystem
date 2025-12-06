using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.SessionAgg;
using MassTransit.Futures.Contracts;
using System.Text.Json;

namespace ClinicApp.Application.DTOs;
public class SessionDTO
{
    public Guid Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public Guid RoomId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public JsonElement? SessionDescription { get; set; }


    public DoctorDTO? doctorDTO { get; set; }
    public RoomDTO? roomDTO { get; set; }
    public PatientDTO? patientDTO { get; set; }
}

public class PatientDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}