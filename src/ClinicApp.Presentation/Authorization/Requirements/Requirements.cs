﻿// Session-related
using ClinicApp.Domain.Common;
using Microsoft.AspNetCore.Authorization;


/// <summary>
/// requirement for doctors and patients to only see their sessions 
/// </summary>
/// <param name="allowedRoles"> roles to view sessions even if they were not their own</param>
public record CanView(UserRole[] allowedRoles) : IAuthorizationRequirement;
public record CanViewSessions(UserRole[] allowedRoles) : CanView(allowedRoles);
public record CanViewSessionStatus : IAuthorizationRequirement;
public record CanViewSessionDetails : IAuthorizationRequirement;
public record CanViewSessionStateHistory : IAuthorizationRequirement;
public record CanEditSession : IAuthorizationRequirement;
public record CanCancelSession : IAuthorizationRequirement;
public record CanModifySessionRequirement : IAuthorizationRequirement;
public record CanAddSessionRequirement : CanModifySessionRequirement;
public record CanUpdateSessionRequirement : CanModifySessionRequirement;

// Room-related
public record CanViewRooms : IAuthorizationRequirement;
public record CanManageRooms : IAuthorizationRequirement;

// Doctor-related
public record CanViewDoctorsInfo : IAuthorizationRequirement;
public record CanViewDoctorWorkingTime(UserRole[] allowedRoles) : CanView(allowedRoles);
public record CanViewDoctorTimesOff(UserRole[] allowedRoles) : CanView(allowedRoles);
public record CanViewDoctorAvailability : IAuthorizationRequirement; // secretary's limited view

// Admin-related
public record CanManageUsers : IAuthorizationRequirement;
public record CanManageSystemSettings : IAuthorizationRequirement;

//For more details go to /src/ClinicApp.Presentation/Policies.md