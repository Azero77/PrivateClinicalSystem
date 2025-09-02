namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class RoomDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DoctorDataModel Docotor { get; set; } = null!;

}