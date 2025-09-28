using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddRiverAndGroundwaterSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroundwaterSites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiteEngName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UgwDistrictName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Township = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Statusofuse = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Twd97Lon = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Twd97Lat = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Twd97Tm2X = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    Twd97Tm2Y = table.Column<decimal>(type: "decimal(12,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundwaterSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringSites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiteEngName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Township = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Basin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    River = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Twd97Lon = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Twd97Lat = table.Column<decimal>(type: "decimal(12,8)", nullable: true),
                    Twd97Tm2X = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    Twd97Tm2Y = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    Statusofuse = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroundwaterSiteSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemEngName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemEngAbbreviation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemValue = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    ItemUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroundwaterSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundwaterSiteSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundwaterSiteSamples_GroundwaterSites_GroundwaterSiteId",
                        column: x => x.GroundwaterSiteId,
                        principalTable: "GroundwaterSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringSiteSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemEngName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemEngAbbreviation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemValue = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    ItemUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonitoringSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringSiteSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringSiteSamples_MonitoringSites_MonitoringSiteId",
                        column: x => x.MonitoringSiteId,
                        principalTable: "MonitoringSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroundwaterSites_SiteId",
                table: "GroundwaterSites",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroundwaterSiteSamples_GroundwaterSiteId",
                table: "GroundwaterSiteSamples",
                column: "GroundwaterSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundwaterSiteSamples_ItemName",
                table: "GroundwaterSiteSamples",
                column: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_GroundwaterSiteSamples_SampleDate",
                table: "GroundwaterSiteSamples",
                column: "SampleDate");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringSites_SiteId",
                table: "MonitoringSites",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringSiteSamples_ItemName",
                table: "MonitoringSiteSamples",
                column: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringSiteSamples_MonitoringSiteId",
                table: "MonitoringSiteSamples",
                column: "MonitoringSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringSiteSamples_SampleDate",
                table: "MonitoringSiteSamples",
                column: "SampleDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroundwaterSiteSamples");

            migrationBuilder.DropTable(
                name: "MonitoringSiteSamples");

            migrationBuilder.DropTable(
                name: "GroundwaterSites");

            migrationBuilder.DropTable(
                name: "MonitoringSites");
        }
    }
}
