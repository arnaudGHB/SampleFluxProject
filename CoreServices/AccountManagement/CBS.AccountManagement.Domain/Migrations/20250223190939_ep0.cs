using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ep0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalBranchId",
                table: "EntryTempData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsInterBranchTransaction",
                table: "EntryTempData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalBranchId",
                table: "EntryTempData");

            migrationBuilder.DropColumn(
                name: "IsInterBranchTransaction",
                table: "EntryTempData");
        }
    }
}
