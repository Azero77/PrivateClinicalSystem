namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class RoomDataModel : DataModel
{
    public string Name { get; set; } = string.Empty;
    public DoctorDataModel? Docotor { get; set; }

}
