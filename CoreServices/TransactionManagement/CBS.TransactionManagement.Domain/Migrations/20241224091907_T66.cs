using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T66 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShareCMoney",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShareCMoney",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMShareCMoney",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShareCMoney",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShareCMoney",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShareCMoney",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShareCMoney",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMShareCMoney",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShareCMoney",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShareCMoney",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCamCCULShareCMoneyTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdDestinationCMoneyTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdFluxAndPTMShareCMoneyTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdHeadOfficeShareCMoneyTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdSourceCMoneyTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShareCMoney",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShareCMoney",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMShareCMoney",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShareCMoney",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShareCMoney",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CamCCULShareCMoney",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShareCMoney",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMShareCMoney",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShareCMoney",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShareCMoney",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "CamCCULShareCMoney",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShareCMoney",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMShareCMoney",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShareCMoney",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShareCMoney",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCamCCULShareCMoneyTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdDestinationCMoneyTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdFluxAndPTMShareCMoneyTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdHeadOfficeShareCMoneyTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdSourceCMoneyTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CamCCULShareCMoney",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShareCMoney",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMShareCMoney",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShareCMoney",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShareCMoney",
                table: "CashDepositParameters");
        }
    }
}
