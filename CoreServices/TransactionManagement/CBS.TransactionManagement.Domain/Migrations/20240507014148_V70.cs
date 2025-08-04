using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V70 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters");

            migrationBuilder.RenameColumn(
                name: "DepositFeeFlat",
                table: "CashDepositParameters",
                newName: "DepositFormFee");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_A",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_B",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_C",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_D",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_E",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_F",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_G",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_A",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_B",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_C",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_D",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_E",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_F",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_G",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_A",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_B",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_C",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_D",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_E",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_F",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_G",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositFormFee",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForDepositFee",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForDepositFormFee",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationPeriodInDays",
                table: "WithdrawalParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_A",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_B",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_C",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_D",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_E",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_F",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCharge_G",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_A",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_B",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_C",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_D",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_E",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_F",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountFrom_G",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_A",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_B",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_C",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_D",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_E",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_F",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTo_G",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForDepositFee",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForDepositFormFee",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountCharge_A",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_B",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_C",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_D",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_E",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_F",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_G",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_A",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_B",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_C",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_D",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_E",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_F",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_G",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_A",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_B",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_C",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_D",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_E",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_F",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_G",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "DepositFormFee",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForDepositFee",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForDepositFormFee",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "NotificationPeriodInDays",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_A",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_B",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_C",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_D",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_E",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_F",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountCharge_G",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_A",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_B",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_C",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_D",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_E",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_F",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountFrom_G",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_A",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_B",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_C",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_D",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_E",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_F",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "AmountTo_G",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForDepositFee",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForDepositFormFee",
                table: "CashDepositParameters");

            migrationBuilder.RenameColumn(
                name: "DepositFormFee",
                table: "CashDepositParameters",
                newName: "DepositFeeFlat");

            migrationBuilder.AddColumn<int>(
                name: "NotificationPeriodInMonths",
                table: "WithdrawalParameters",
                type: "int",
                nullable: true);
        }
    }
}
