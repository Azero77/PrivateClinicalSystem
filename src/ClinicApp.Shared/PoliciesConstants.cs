namespace ClinicApp.Shared;

public static class PoliciesConstants
{
    //Session-Related
    public const string CanViewOwnSessionsPolicy = "CanViewOwnSessions";
    public const string CanViewAllSessionsPolicy = "CanViewAllSessions"; 
    public const string CanViewSessionStatus = "CanViewSessionStatus";  
    public const string CanViewSessionHistory = "CanViewSessionHistory";  
    public const string CanViewSessionDetails = "CanViewSessionDetails";  
    public const string CanEditSessionDetails = "CanEditSessionDetails";
    public const string CanDeleteSession = "CanDeleteSession";

    // Room-related
    public const string CanViewRooms = "CanViewRooms";
    public const string CanManageRooms = "CanManageRooms";

    // Doctor-related
    public const string CanViewDoctorsInfo = "CanViewDoctorsInfo";
    public const string CanViewDoctorWorkingTime = "CanViewDoctorWorkingTime";
    public const string CanViewDoctorTimesOff = "CanViewDoctorTimesOff";
    public const string CanViewDoctorAvailability = "CanViewDoctorAvailability";

    // Admin-related
    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageSystemSettings = "CanManageSystemSettings";

    //Patient-related
    public const string CanViewPatient = "CanViewPatient";
}
