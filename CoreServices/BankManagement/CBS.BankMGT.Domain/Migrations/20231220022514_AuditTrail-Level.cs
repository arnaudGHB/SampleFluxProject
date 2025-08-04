using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AuditTrailLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "AuditTrails");
        }
    }
}
