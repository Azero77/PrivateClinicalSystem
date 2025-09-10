namespace ClinicApp.Application.QueryTypes;

public class RoomQueryType : QueryType
{
    public string Name { get; set; } = string.Empty;
    public DoctorQueryType Docotor { get; set; } = null!;

}
