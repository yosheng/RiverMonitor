using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIrrigationAgencyStationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IrrigationAgencyStations_Name",
                table: "IrrigationAgencyStations");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencyStations_Name_IrrigationAgencyId",
                table: "IrrigationAgencyStations",
                columns: new[] { "Name", "IrrigationAgencyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IrrigationAgencyStations_Name_IrrigationAgencyId",
                table: "IrrigationAgencyStations");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencyStations_Name",
                table: "IrrigationAgencyStations",
                column: "Name",
                unique: true);
        }
    }
}
