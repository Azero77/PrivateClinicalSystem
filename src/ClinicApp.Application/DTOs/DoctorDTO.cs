
namespace ClinicApp.Application.DTOs;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
}
