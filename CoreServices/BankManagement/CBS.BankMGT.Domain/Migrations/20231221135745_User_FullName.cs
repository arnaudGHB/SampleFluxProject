using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class User_FullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankchID",
                table: "AuditTrails",
                newName: "UserID");

            migrationBuilder.AddColumn<string>(
                name: "BranchID",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AuditTrails");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "AuditTrails",
                newName: "BankchID");
        }
    }
}
