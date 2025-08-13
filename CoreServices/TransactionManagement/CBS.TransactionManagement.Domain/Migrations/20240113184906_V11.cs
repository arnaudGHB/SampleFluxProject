using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialAmount",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "TellerProvisioningAccounts");

            migrationBuilder.DropColumn(
                name: "InitialAmount",
                table: "TellerProvisioningAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InitialAmount",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "TellerProvisioningAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialAmount",
                table: "TellerProvisioningAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
