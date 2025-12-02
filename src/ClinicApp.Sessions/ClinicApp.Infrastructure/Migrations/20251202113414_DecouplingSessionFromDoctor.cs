using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DecouplingSessionFromDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Rooms_RoomId",
                schema: "domain",
                table: "Doctors");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Rooms_RoomId",
                schema: "domain",
                table: "Doctors",
                column: "RoomId",
                principalSchema: "domain",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Rooms_RoomId",
                schema: "domain",
                table: "Doctors");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Rooms_RoomId",
                schema: "domain",
                table: "Doctors",
                column: "RoomId",
                principalSchema: "domain",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
