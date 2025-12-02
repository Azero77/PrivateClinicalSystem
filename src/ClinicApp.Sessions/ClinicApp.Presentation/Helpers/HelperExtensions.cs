using ClinicApp.Domain.Common;
using System.Security.Claims;

namespace ClinicApp.Presentation.Helpers;

public static class HelperExtensions
{
    public static UserRole GetRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirst("role");
        if (role is null)
            throw new ArgumentException("User is not authenticated");
        return Enum.Parse<UserRole>(role.Value.ToString());
    }
}
