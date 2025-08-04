using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PD2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TrialBalanceReferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TrialBalanceFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TrialBalance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TransactionTrackers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TransactionReversalRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TransactionReversalRequestData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TransactionData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TrailBalanceUplouds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "TellerDailyProvisions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "StatementModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ReportDownloads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ProductAccountingBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "PostedEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "OrganizationalUnits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "OperationEventAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "OperationEvent",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "EntryTempData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "DocumentTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "DocumentReferenceCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "DepositNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CorrespondingMappings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ChartOfAccountManagementPositions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ChartOfAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CashReplenishments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CashMovementTrackingConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CashMovementTracker",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BudgetPeriods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BudgetItemDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BudgetCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Budget",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BankTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BalanceSheetAccountDtos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountingRules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountingRuleEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountingEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AccountCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TrialBalanceReferences");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TrialBalanceFiles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TrialBalance");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TransactionTrackers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TransactionReversalRequests");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TransactionReversalRequestData");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TransactionData");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TrailBalanceUplouds");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "TellerDailyProvisions");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "StatementModels");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ReportDownloads");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ProductAccountingBook");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "PostedEntries");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "OrganizationalUnits");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "OperationEventAttributes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "OperationEvent");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "EntryTempData");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "DocumentReferenceCodes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "DepositNotifications");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CorrespondingMappings");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ChartOfAccountManagementPositions");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ChartOfAccount");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CashReplenishments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CashMovementTrackingConfigurations");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CashMovementTracker");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BudgetPeriods");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BudgetItemDetails");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BudgetCategory");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BalanceSheetAccountDtos");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountTypes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountPolicies");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountingRules");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountingRuleEntries");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountingEntries");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountClasses");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AccountCategory");
        }
    }
}
