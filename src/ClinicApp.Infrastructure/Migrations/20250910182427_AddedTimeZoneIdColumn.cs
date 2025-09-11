using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeZoneIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkingTime_WorkingHours_TimeZoneId",
                schema: "domain",
                table: "Doctors",
                newName: "TimeZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeZoneId",
                schema: "domain",
                table: "Doctors",
                newName: "WorkingTime_WorkingHours_TimeZoneId");
        }
    }
}
