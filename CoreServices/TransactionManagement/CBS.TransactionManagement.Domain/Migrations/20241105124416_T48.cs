using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActivateFor3PPApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActivateForMobileApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActivateSavingWithdrawalNotificationForMobileApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformCashOut3PP",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformCashOutMobileApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformCashin3PP",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformCashinMobileApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformTransfer3PP",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPeformTransferMobileApp",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SavingProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivateFor3PPApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ActivateForMobileApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ActivateSavingWithdrawalNotificationForMobileApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformCashOut3PP",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformCashOutMobileApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformCashin3PP",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformCashinMobileApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformTransfer3PP",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanPeformTransferMobileApp",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SavingProducts");
        }
    }
}
