namespace ClinicApp.Presentation.Authorization.Policies;

public static class PoliciesConstants
{
    //Session-Related
    public static string CanViewOwnSessionsPolicy = "CanViewOwnSessions";
    public static string CanViewAllSessionsPolicy = "CanViewAllSessions";  
    public static string CanViewSessionStatus = "CanViewSessionStatus";  
    public static string CanViewSessionHistory = "CanViewSessionHistory";  
    public static string CanViewSessionDetails = "CanViewSessionDetails";  
    public static string CanEditSessionDetails = "CanEditSessionDetails";
    public static string CanDeleteSession = "CanDeleteSession";

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
}
