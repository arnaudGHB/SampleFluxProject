using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PD5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountOwner",
                table: "DepositNotifications");

            migrationBuilder.AddColumn<string>(
                name: "CorrepondingBranchId",
                table: "DepositNotifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrepondingBranchId",
                table: "DepositNotifications");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountOwner",
                table: "DepositNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
