using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventCode",
                table: "Tellers",
                newName: "TopupMobileMoneyLedgerAccountNumberTo");

            migrationBuilder.AddColumn<string>(
                name: "MobileMoneyAlertMessageInEnglish",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileMoneyAlertMessageInFrench",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileMoneyFloatNumber",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MobileMoneyMaximumBalanceAlertLevel",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MobileMoneyMinimumBalanceAlertLevel",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MobileMoneyUserKeepingThePhone",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventCode",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberToRecieveAlert",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopupMobileMoneyLedgerAccountNumberFrom",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualBalance",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MobileMoneyCashTopups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperatorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestInitiatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestApprovalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileMoneyTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileMoneyMemberReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TellerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileMoneyCashTopups", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileMoneyCashTopups");

            migrationBuilder.DropColumn(
                name: "MobileMoneyAlertMessageInEnglish",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MobileMoneyAlertMessageInFrench",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MobileMoneyFloatNumber",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MobileMoneyMaximumBalanceAlertLevel",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MobileMoneyMinimumBalanceAlertLevel",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MobileMoneyUserKeepingThePhone",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "OperationEventCode",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberToRecieveAlert",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "TopupMobileMoneyLedgerAccountNumberFrom",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "ActualBalance",
                table: "CashCeillingMovements");

            migrationBuilder.RenameColumn(
                name: "TopupMobileMoneyLedgerAccountNumberTo",
                table: "Tellers",
                newName: "EventCode");
        }
    }
}
