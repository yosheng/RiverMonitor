using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollutionSites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Township = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SiteType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteUse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pollutants = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ControlType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LandLots = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Dtmx = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Dtmy = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollutionSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WastewaterPermits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmsNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UniformNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PermitNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermitStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PermitEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PermitType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermittedWaterVolume = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OutletId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutletTm2x = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    OutletTm2y = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    OutletLongitude = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    OutletLatitude = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    OutletWaterType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WastewaterPermits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnouncementNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnnouncementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSoilPollutionZone = table.Column<bool>(type: "bit", nullable: false),
                    IsGroundwaterPollutionZone = table.Column<bool>(type: "bit", nullable: false),
                    PollutionSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteAnnouncements_PollutionSites_PollutionSiteId",
                        column: x => x.PollutionSiteId,
                        principalTable: "PollutionSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollutantEmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmissionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmissionEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmissionWaterVolume = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    EmissionWaterUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmissionItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmissionValue = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    EmissionUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalItemValue = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    TotalItemUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WastewaterPermitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollutantEmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollutantEmissions_WastewaterPermits_WastewaterPermitId",
                        column: x => x.WastewaterPermitId,
                        principalTable: "WastewaterPermits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollutantEmissions_EmissionItemName",
                table: "PollutantEmissions",
                column: "EmissionItemName");

            migrationBuilder.CreateIndex(
                name: "IX_PollutantEmissions_EmissionStartDate",
                table: "PollutantEmissions",
                column: "EmissionStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_PollutantEmissions_WastewaterPermitId",
                table: "PollutantEmissions",
                column: "WastewaterPermitId");

            migrationBuilder.CreateIndex(
                name: "IX_PollutionSites_SiteId",
                table: "PollutionSites",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteAnnouncements_AnnouncementNo",
                table: "SiteAnnouncements",
                column: "AnnouncementNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteAnnouncements_PollutionSiteId",
                table: "SiteAnnouncements",
                column: "PollutionSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_WastewaterPermits_EmsNo",
                table: "WastewaterPermits",
                column: "EmsNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollutantEmissions");

            migrationBuilder.DropTable(
                name: "SiteAnnouncements");

            migrationBuilder.DropTable(
                name: "WastewaterPermits");

            migrationBuilder.DropTable(
                name: "PollutionSites");
        }
    }
}
