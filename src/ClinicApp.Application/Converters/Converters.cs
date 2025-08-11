using ClinicApp.Application.DTOs;
using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Application.Converters;
public static class Converters
{
    public static Doctor ToDoctor(this DoctorWithSessionsDTO dto)
    {
        return new Doctor(dto.Id,
            dto.WorkingDays,
            dto.WorkingHours
            );
    }
}
