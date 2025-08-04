using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralDailyDashboards",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfCashIn = table.Column<int>(type: "int", nullable: false),
                    NumberOfCashOut = table.Column<int>(type: "int", nullable: false),
                    TotalCashInAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCashOutAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewMembers = table.Column<int>(type: "int", nullable: false),
                    ClosedAccounts = table.Column<int>(type: "int", nullable: false),
                    ActiveAccounts = table.Column<int>(type: "int", nullable: false),
                    DormantAccounts = table.Column<int>(type: "int", nullable: false),
                    LoanDisbursements = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanRepayments = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceFeesCollected = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrdinaryShares = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreferenceShares = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Savings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deposits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashInHand57 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashInHand56 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MTNMobileMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrangeMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyCollectionCashOut = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyCollectionCashIn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MomocashCollection = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Transfer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrimaryTillOpenOfDayBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTillTillOpenOfDayBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTillBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrimaryBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDailyDashboards", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralDailyDashboards");
        }
    }
}
