using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refresher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientDataModelId",
                schema: "domain",
                table: "Sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PatientDataModelId",
                schema: "domain",
                table: "Sessions",
                column: "PatientDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                schema: "domain",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                schema: "domain",
                table: "Doctors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Patients_PatientDataModelId",
                schema: "domain",
                table: "Sessions",
                column: "PatientDataModelId",
                principalSchema: "domain",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Patients_PatientDataModelId",
                schema: "domain",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_PatientDataModelId",
                schema: "domain",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                schema: "domain",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                schema: "domain",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PatientDataModelId",
                schema: "domain",
                table: "Sessions");
        }
    }
}
