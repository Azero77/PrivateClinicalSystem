﻿using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Application.DTOs;

public class DoctorWithSessionsDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public WorkingDays WorkingDays { get; set; }
    public WorkingHours WorkingHours { get; set; } = null!;
    public IReadOnlyCollection<Session> Sessions { get; set; } = new List<Session>().AsReadOnly();
}


