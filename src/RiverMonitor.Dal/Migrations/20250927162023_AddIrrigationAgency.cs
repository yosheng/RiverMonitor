using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddIrrigationAgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings");

            migrationBuilder.RenameTable(
                name: "SystemSettings",
                newName: "SystemSetting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemSetting",
                table: "SystemSetting",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IrrigationAgencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpenUnitId = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    WorkStationUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationAgencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationAgencyStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Twd97Lon = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Twd97Lat = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IrrigationAgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationAgencyStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrrigationAgencyStations_IrrigationAgencies_IrrigationAgencyId",
                        column: x => x.IrrigationAgencyId,
                        principalTable: "IrrigationAgencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencies_OpenUnitId",
                table: "IrrigationAgencies",
                column: "OpenUnitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencyStations_IrrigationAgencyId",
                table: "IrrigationAgencyStations",
                column: "IrrigationAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencyStations_Name",
                table: "IrrigationAgencyStations",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrrigationAgencyStations");

            migrationBuilder.DropTable(
                name: "IrrigationAgencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemSetting",
                table: "SystemSetting");

            migrationBuilder.RenameTable(
                name: "SystemSetting",
                newName: "SystemSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings",
                column: "Id");
        }
    }
}
