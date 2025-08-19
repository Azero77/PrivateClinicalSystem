using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Application.DTOs;

public class DoctorWithSessionsDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> WorkingDays { get; set; } = new(); // {sat,sun,wedoctor...}
    public List<bool> WorkingDaysBit { get; set; } = new();
    public WorkingHours WorkingHours { get; set; } = null!;
    public IReadOnlyCollection<Session> Sessions { get; set; } = new List<Session>().AsReadOnly();
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
}