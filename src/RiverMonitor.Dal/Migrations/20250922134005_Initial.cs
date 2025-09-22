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
                name: "pollution_sites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    site_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    site_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    county = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    township = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    site_type = table.Column<string>(type: "TEXT", nullable: true),
                    site_use = table.Column<string>(type: "TEXT", nullable: true),
                    pollutants = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    control_type = table.Column<string>(type: "TEXT", nullable: true),
                    site_area = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    land_lots = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    dtmx = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    dtmy = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(18, 15)", nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(18, 15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pollution_sites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wastewater_permits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ems_no = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    facility_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    uniform_no = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    permit_no = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    permit_start_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    permit_end_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    permit_type = table.Column<string>(type: "TEXT", nullable: true),
                    permitted_water_volume = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    outlet_id = table.Column<string>(type: "TEXT", nullable: true),
                    outlet_tm2x = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    outlet_tm2y = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    outlet_longitude = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    outlet_latitude = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    outlet_water_type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wastewater_permits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_announcements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    announcement_no = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    announcement_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    content = table.Column<string>(type: "TEXT", nullable: true),
                    is_soil_pollution_zone = table.Column<bool>(type: "INTEGER", nullable: false),
                    is_groundwater_pollution_zone = table.Column<bool>(type: "INTEGER", nullable: false),
                    pollution_site_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_site_announcements", x => x.id);
                    table.ForeignKey(
                        name: "fk_site_announcements_pollution_sites_pollution_site_id",
                        column: x => x.pollution_site_id,
                        principalTable: "pollution_sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pollutant_emissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    emission_start_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    emission_end_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    emission_water_volume = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    emission_water_unit = table.Column<string>(type: "TEXT", nullable: true),
                    emission_item_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    emission_value = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    emission_unit = table.Column<string>(type: "TEXT", nullable: true),
                    total_item_value = table.Column<decimal>(type: "decimal(18, 10)", nullable: true),
                    total_item_unit = table.Column<string>(type: "TEXT", nullable: true),
                    wastewater_permit_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pollutant_emissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_pollutant_emissions_wastewater_permits_wastewater_permit_id",
                        column: x => x.wastewater_permit_id,
                        principalTable: "wastewater_permits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pollutant_emissions_emission_item_name",
                table: "pollutant_emissions",
                column: "emission_item_name");

            migrationBuilder.CreateIndex(
                name: "ix_pollutant_emissions_emission_start_date",
                table: "pollutant_emissions",
                column: "emission_start_date");

            migrationBuilder.CreateIndex(
                name: "ix_pollutant_emissions_wastewater_permit_id",
                table: "pollutant_emissions",
                column: "wastewater_permit_id");

            migrationBuilder.CreateIndex(
                name: "ix_pollution_sites_site_id",
                table: "pollution_sites",
                column: "site_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_announcements_announcement_no",
                table: "site_announcements",
                column: "announcement_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_announcements_pollution_site_id",
                table: "site_announcements",
                column: "pollution_site_id");

            migrationBuilder.CreateIndex(
                name: "ix_wastewater_permits_ems_no",
                table: "wastewater_permits",
                column: "ems_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pollutant_emissions");

            migrationBuilder.DropTable(
                name: "site_announcements");

            migrationBuilder.DropTable(
                name: "wastewater_permits");

            migrationBuilder.DropTable(
                name: "pollution_sites");
        }
    }
}
