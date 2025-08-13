using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowInterbranchWithdrawal",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargesToAppliedConsideringClosingTheAccount",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargesToAppliedWithinNormalPeriod",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfiguredForShareing",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawalFromSavingAccount",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumRateToConsiderAccountClossed",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumWithdrawalRateToConsiderAccountOK",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfiguredForShareing",
                table: "TransferParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InterDepositOperationType",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfiguredForShareing",
                table: "CashDepositParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowInterbranchWithdrawal",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "ChargesToAppliedConsideringClosingTheAccount",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "ChargesToAppliedWithinNormalPeriod",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsConfiguredForShareing",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsWithdrawalFromSavingAccount",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "MaximumRateToConsiderAccountClossed",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "MinimumWithdrawalRateToConsiderAccountOK",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsConfiguredForShareing",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "InterDepositOperationType",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "IsConfiguredForShareing",
                table: "CashDepositParameters");
        }
    }
}
