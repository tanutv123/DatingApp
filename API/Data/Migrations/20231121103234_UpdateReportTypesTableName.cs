using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportTypesTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportType_ReportTypeId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType");

            migrationBuilder.RenameTable(
                name: "ReportType",
                newName: "ReportTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportTypes_ReportTypeId",
                table: "Reports",
                column: "ReportTypeId",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportTypes_ReportTypeId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportTypes",
                table: "ReportTypes");

            migrationBuilder.RenameTable(
                name: "ReportTypes",
                newName: "ReportType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportType",
                table: "ReportType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportType_ReportTypeId",
                table: "Reports",
                column: "ReportTypeId",
                principalTable: "ReportType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
