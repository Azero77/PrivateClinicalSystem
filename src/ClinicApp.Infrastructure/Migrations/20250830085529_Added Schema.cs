using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Patient_PatientId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.EnsureSchema(
                name: "domain");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "Sessions",
                newSchema: "domain");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Rooms",
                newSchema: "domain");

            migrationBuilder.RenameTable(
                name: "Doctors",
                newName: "Doctors",
                newSchema: "domain");

            migrationBuilder.RenameTable(
                name: "Doctor_TimesOff",
                newName: "Doctor_TimesOff",
                newSchema: "domain");

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "domain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Patients_PatientId",
                schema: "domain",
                table: "Sessions",
                column: "PatientId",
                principalSchema: "domain",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Patients_PatientId",
                schema: "domain",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "domain");

            migrationBuilder.RenameTable(
                name: "Sessions",
                schema: "domain",
                newName: "Sessions");

            migrationBuilder.RenameTable(
                name: "Rooms",
                schema: "domain",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Doctors",
                schema: "domain",
                newName: "Doctors");

            migrationBuilder.RenameTable(
                name: "Doctor_TimesOff",
                schema: "domain",
                newName: "Doctor_TimesOff");

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Patient_PatientId",
                table: "Sessions",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id");
        }
    }
}
