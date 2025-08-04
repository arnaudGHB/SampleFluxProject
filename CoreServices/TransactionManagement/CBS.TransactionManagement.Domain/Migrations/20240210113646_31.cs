using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestExpenseAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdSavingAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountId",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountId",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountId",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "EntryFeeParameters");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "CashDepositParameters");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountId",
                table: "WithdrawalParameters",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountId",
                table: "TransferParameters",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "CashDepositParameters",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountId",
                table: "CashDepositParameters",
                newName: "BankId");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "TransferParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdClossingFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestSavingAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestSavingExpenseAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdManagementFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdPricipalSavingAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdRepoeningFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdSavingFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdTransferFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdWithrawalFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankId",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "TransferParameters");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdClossingFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestSavingAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestSavingExpenseAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdManagementFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdPricipalSavingAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdRepoeningFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdSavingFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdTransferFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdWithrawalFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ReopenFeeParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ManagementFeeParameters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "EntryFeeParameters");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "WithdrawalParameters",
                newName: "ChartOfAccountId");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "TransferParameters",
                newName: "ChartOfAccountId");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "CashDepositParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "CashDepositParameters",
                newName: "ChartOfAccountId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "TransferParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "TransferParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestExpenseAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdSavingAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountId",
                table: "ReopenFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "ReopenFeeParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "ReopenFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountId",
                table: "ManagementFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "ManagementFeeParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "ManagementFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountId",
                table: "EntryFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "EntryFeeParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "EntryFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "CashDepositParameters",
                type: "bit",
                nullable: true);
        }
    }
}
