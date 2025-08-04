using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "CashDepositParameters");

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "TransferParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "ReopenFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "ReopenFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "ReopenFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "ReopenFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "ManagementFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "ManagementFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "ManagementFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "ManagementFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "EntryFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "EntryFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "EntryFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "EntryFeeParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchOfficeShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartnerShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBrachOfficeShare",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "DestinationBranchOfficeShare",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "HeadOfficeShare",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "PartnerShare",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "SourceBrachOfficeShare",
                table: "CashDepositParameters");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "TransferParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "ReopenFeeParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "ManagementFeeParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "EntryFeeParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
