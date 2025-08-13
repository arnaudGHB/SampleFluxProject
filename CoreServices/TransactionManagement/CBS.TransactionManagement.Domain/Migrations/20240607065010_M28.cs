using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountantAmountCollected",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "AccountantCloseOfDayComment",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "AccountantComment",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "AccountantConfirmationStatus",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "AccountantUserID",
                table: "PrimaryTellerProvisioningHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccountantAmountCollected",
                table: "PrimaryTellerProvisioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AccountantCloseOfDayComment",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountantComment",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountantConfirmationStatus",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountantUserID",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
