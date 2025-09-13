// Session-related
using Microsoft.AspNetCore.Authorization;

public record CanViewOwnSessions : IAuthorizationRequirement;
public record CanViewAllSessions : IAuthorizationRequirement;
public record CanViewSessionStatus : IAuthorizationRequirement;
public record CanViewSessionDetails : IAuthorizationRequirement;
public record CanViewSessionStateHistory : IAuthorizationRequirement;
public record CanEditSession : IAuthorizationRequirement;
public record CanCancelSession : IAuthorizationRequirement;

// Room-related
public record CanViewRooms : IAuthorizationRequirement;
public record CanManageRooms : IAuthorizationRequirement;

// Doctor-related
public record CanViewDoctorsInfo : IAuthorizationRequirement;
public record CanViewDoctorWorkingTime : IAuthorizationRequirement;
public record CanViewDoctorTimesOff : IAuthorizationRequirement;
public record CanViewDoctorAvailability : IAuthorizationRequirement; // secretary's limited view

// Admin-related
public record CanManageUsers : IAuthorizationRequirement;
public record CanManageSystemSettings : IAuthorizationRequirement;