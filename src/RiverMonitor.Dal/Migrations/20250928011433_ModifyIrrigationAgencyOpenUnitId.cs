using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIrrigationAgencyOpenUnitId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IrrigationAgencies_OpenUnitId",
                table: "IrrigationAgencies");

            migrationBuilder.AlterColumn<string>(
                name: "OpenUnitId",
                table: "IrrigationAgencies",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencies_OpenUnitId",
                table: "IrrigationAgencies",
                column: "OpenUnitId",
                unique: true,
                filter: "[OpenUnitId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IrrigationAgencies_OpenUnitId",
                table: "IrrigationAgencies");

            migrationBuilder.AlterColumn<string>(
                name: "OpenUnitId",
                table: "IrrigationAgencies",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationAgencies_OpenUnitId",
                table: "IrrigationAgencies",
                column: "OpenUnitId",
                unique: true);
        }
    }
}
