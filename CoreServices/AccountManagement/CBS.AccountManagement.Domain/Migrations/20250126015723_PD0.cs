using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PD0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoubleValidationNeeded",
                table: "AccountingRules",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoubleValidationNeeded",
                table: "AccountingRules");
        }
    }
}
