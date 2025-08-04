using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PD1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostingSource",
                table: "PostedEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountingEventId",
                table: "EntryTempData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsdoubbleValidationRequired",
                table: "EntryTempData",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositingSource",
                table: "EntryTempData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingSource",
                table: "PostedEntries");

            migrationBuilder.DropColumn(
                name: "AccountingEventId",
                table: "EntryTempData");

            migrationBuilder.DropColumn(
                name: "IsdoubbleValidationRequired",
                table: "EntryTempData");

            migrationBuilder.DropColumn(
                name: "PositingSource",
                table: "EntryTempData");
        }
    }
}
