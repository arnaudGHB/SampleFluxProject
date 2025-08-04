using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class B2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "AuditTrails");
        }
    }
}
