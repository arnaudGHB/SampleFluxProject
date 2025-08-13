using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "SubTellerProvioningHistories",
                newName: "SubTellerComment");

            migrationBuilder.RenameColumn(
                name: "BalanceAtHand",
                table: "SubTellerProvioningHistories",
                newName: "CashAtHand");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "PrimaryTellerProvisioningHistories",
                newName: "PrimaryTellerComment");

            migrationBuilder.RenameColumn(
                name: "BalanceAtHand",
                table: "PrimaryTellerProvisioningHistories",
                newName: "CashAtHand");

            migrationBuilder.AddColumn<string>(
                name: "ClossedStatus",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTellerComment",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTellerConfirmationStatus",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<string>(
                name: "ClossedStatus",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClossedStatus",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "PrimaryTellerComment",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "PrimaryTellerConfirmationStatus",
                table: "SubTellerProvioningHistories");

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

            migrationBuilder.DropColumn(
                name: "ClossedStatus",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.RenameColumn(
                name: "SubTellerComment",
                table: "SubTellerProvioningHistories",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "CashAtHand",
                table: "SubTellerProvioningHistories",
                newName: "BalanceAtHand");

            migrationBuilder.RenameColumn(
                name: "PrimaryTellerComment",
                table: "PrimaryTellerProvisioningHistories",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "CashAtHand",
                table: "PrimaryTellerProvisioningHistories",
                newName: "BalanceAtHand");
        }
    }
}
