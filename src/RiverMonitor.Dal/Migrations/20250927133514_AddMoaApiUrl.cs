using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverMonitor.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddMoaApiUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemSetting",
                columns: new[] { "Id", "Description", "Key", "Value" },
                values: new object[] { 3, null, "Endpoint:MoaApi", "https://data.moa.gov.tw" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSetting",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
