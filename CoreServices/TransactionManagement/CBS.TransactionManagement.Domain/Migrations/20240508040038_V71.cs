using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationPeriodInDays",
                table: "WithdrawalParameters",
                newName: "NumberOfDayToNotifyBeforeWithdrawal");

            migrationBuilder.RenameColumn(
                name: "IsWithdrawalFromSavingAccount",
                table: "WithdrawalParameters",
                newName: "MustNotifyOnWithdrawal");

            migrationBuilder.RenameColumn(
                name: "DepositFormFee",
                table: "WithdrawalParameters",
                newName: "WithdrawalFormCharge");

            migrationBuilder.AddColumn<decimal>(
                name: "CloseOfAccountCharge",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawalDoneOnthisAccount",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawalChargeWithoutNotification",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CloseOfAccountCharge",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FormCharge",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OperationCharge",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawalChargeWithoutNotification",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseOfAccountCharge",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsWithdrawalDoneOnthisAccount",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "WithdrawalChargeWithoutNotification",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "CloseOfAccountCharge",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FormCharge",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OperationCharge",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawalChargeWithoutNotification",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "WithdrawalFormCharge",
                table: "WithdrawalParameters",
                newName: "DepositFormFee");

            migrationBuilder.RenameColumn(
                name: "NumberOfDayToNotifyBeforeWithdrawal",
                table: "WithdrawalParameters",
                newName: "NotificationPeriodInDays");

            migrationBuilder.RenameColumn(
                name: "MustNotifyOnWithdrawal",
                table: "WithdrawalParameters",
                newName: "IsWithdrawalFromSavingAccount");
        }
    }
}
