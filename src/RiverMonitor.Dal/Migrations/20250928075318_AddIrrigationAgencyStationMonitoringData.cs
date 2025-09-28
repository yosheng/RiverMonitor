using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddIrrigationAgencyStationMonitoringData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IrrigationAgencyStationMonitoringData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SampleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WaterTemperatureC = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PhValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ElectricalConductivity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IrrigationAgencyStationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationAgencyStationMonitoringData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrrigationAgencyStationMonitoringData_IrrigationAgencyStations_IrrigationAgencyStationId",
                        column: x => x.IrrigationAgencyStationId,
                        principalTable: "IrrigationAgencyStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencyStationMonitoringData_IrrigationAgencyStationId_SampleDate",
                table: "IrrigationAgencyStationMonitoringData",
                columns: new[] { "IrrigationAgencyStationId", "SampleDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrrigationAgencyStationMonitoringData");
        }
    }
}
