using ClinicApp.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ClinicApp.Identity.Server.Infrastructure.Persistance;

public class UsersDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");
        var roles = GetRoles();
        builder.Entity<ApplicationRole>()
            .HasData(roles);
        base.OnModelCreating(builder);
    }

    private List<ApplicationRole> GetRoles()
    {
        var values = Enum.GetValues<UserRole>();
        var identityRoles = new List<ApplicationRole>();
        foreach (var value in values)
        {
            identityRoles.Add(new ApplicationRole { Id = GetRoleId(value), Name = value.ToString(),NormalizedName = value.ToString().ToUpper()});
        }
        return identityRoles;
    }

    private static Guid GetRoleId(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => Guid.Parse("2c5e174e-3b0e-446f-86af-483d56fd7210"),
            UserRole.Doctor => Guid.Parse("7c5e174e-3b0e-446f-86af-483d56fd7211"),
            UserRole.Patient => Guid.Parse("8c5e174e-3b0e-446f-86af-483d56fd7212"),
            UserRole.Secretary => Guid.Parse("9c5e174e-3b0e-446f-86af-483d56fd7213"),
            // Add a default case to handle any new roles you might add to the enum
            _ => throw new ArgumentOutOfRangeException(nameof(role), $"No Gui defined for role {role}")
        };
    }
}

public class ApplicationRole : IdentityRole<Guid>
{

}