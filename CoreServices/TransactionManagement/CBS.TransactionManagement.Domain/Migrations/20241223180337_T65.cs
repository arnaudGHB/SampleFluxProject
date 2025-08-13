using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T65 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartnerShare",
                table: "TransferParameters",
                newName: "FluxAndPTMShare");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdCommissionAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdHeadOfficeShareTransferCommission");

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShare",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULCommission",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMCommission",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeCommission",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCamCCULShareCashInCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCamCCULShareCashOutCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCamCCULShareTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCashInCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdCashOutCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdFluxAndPTMShareCashInCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdFluxAndPTMShareCashOutCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdFluxAndPTMShareTransferCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdHeadOfficeShareCashInCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdHeadOfficeShareCashOutCommission",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CamCCULShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FluxAndPTMShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CamCCULShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "CamCCULShare",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "CamCCULCommission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMCommission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "HeadOfficeCommission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCamCCULShareCashInCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCamCCULShareCashOutCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCamCCULShareTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCashInCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdCashOutCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdFluxAndPTMShareCashInCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdFluxAndPTMShareCashOutCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdFluxAndPTMShareTransferCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdHeadOfficeShareCashInCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdHeadOfficeShareCashOutCommission",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CamCCULShare",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "FluxAndPTMShare",
                table: "CashDepositParameters");

            migrationBuilder.RenameColumn(
                name: "FluxAndPTMShare",
                table: "TransferParameters",
                newName: "PartnerShare");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdHeadOfficeShareTransferCommission",
                table: "SavingProducts",
                newName: "ChartOfAccountIdCommissionAccount");
        }
    }
}
