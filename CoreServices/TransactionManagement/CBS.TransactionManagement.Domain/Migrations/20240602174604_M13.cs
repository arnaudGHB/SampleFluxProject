using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_SavingProducts_ProductId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedAccounts_Accounts_AccountId",
                table: "BlockedAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOutThirdParty_Accounts_AccountId",
                table: "CashOutThirdParty");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Accounts_AccountId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalNotifications_Accounts_AccountId",
                table: "WithdrawalNotifications");

            migrationBuilder.DropTable(
                name: "TellerProvisioningStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "InUseStatus",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "InUsedByUserId",
                table: "Tellers");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "MemberAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_ProductId",
                table: "MemberAccounts",
                newName: "IX_MemberAccounts_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "DailyTellerId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DailyTellerId",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberAccounts",
                table: "MemberAccounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DailyTellers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisionedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TellerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaximumWithdrawalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumCeilin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_DailyTellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTellers_Tellers_TellerId",
                        column: x => x.TellerId,
                        principalTable: "Tellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubTellerProvioningHistories_DailyTellerId",
                table: "SubTellerProvioningHistories",
                column: "DailyTellerId");

            migrationBuilder.CreateIndex(
                name: "IX_PrimaryTellerProvisioningHistories_DailyTellerId",
                table: "PrimaryTellerProvisioningHistories",
                column: "DailyTellerId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTellers_TellerId",
                table: "DailyTellers",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedAccounts_MemberAccounts_AccountId",
                table: "BlockedAccounts",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CashOutThirdParty_MemberAccounts_AccountId",
                table: "CashOutThirdParty",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccounts_SavingProducts_ProductId",
                table: "MemberAccounts",
                column: "ProductId",
                principalTable: "SavingProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrimaryTellerProvisioningHistories_DailyTellers_DailyTellerId",
                table: "PrimaryTellerProvisioningHistories",
                column: "DailyTellerId",
                principalTable: "DailyTellers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTellerProvioningHistories_DailyTellers_DailyTellerId",
                table: "SubTellerProvioningHistories",
                column: "DailyTellerId",
                principalTable: "DailyTellers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_MemberAccounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_MemberAccounts_AccountId",
                table: "Transfers",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalNotifications_MemberAccounts_AccountId",
                table: "WithdrawalNotifications",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedAccounts_MemberAccounts_AccountId",
                table: "BlockedAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOutThirdParty_MemberAccounts_AccountId",
                table: "CashOutThirdParty");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccounts_SavingProducts_ProductId",
                table: "MemberAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_PrimaryTellerProvisioningHistories_DailyTellers_DailyTellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTellerProvioningHistories_DailyTellers_DailyTellerId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MemberAccounts_AccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_MemberAccounts_AccountId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalNotifications_MemberAccounts_AccountId",
                table: "WithdrawalNotifications");

            migrationBuilder.DropTable(
                name: "DailyTellers");

            migrationBuilder.DropIndex(
                name: "IX_SubTellerProvioningHistories_DailyTellerId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropIndex(
                name: "IX_PrimaryTellerProvisioningHistories_DailyTellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberAccounts",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "DailyTellerId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "DailyTellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.RenameTable(
                name: "MemberAccounts",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_MemberAccounts_ProductId",
                table: "Accounts",
                newName: "IX_Accounts_ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "InUseStatus",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InUsedByUserId",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TellerProvisioningStatuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryTeller = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProvisionedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TellerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIDInChargeOfTeller = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TellerProvisioningStatuses", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_SavingProducts_ProductId",
                table: "Accounts",
                column: "ProductId",
                principalTable: "SavingProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedAccounts_Accounts_AccountId",
                table: "BlockedAccounts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CashOutThirdParty_Accounts_AccountId",
                table: "CashOutThirdParty",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Accounts_AccountId",
                table: "Transfers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalNotifications_Accounts_AccountId",
                table: "WithdrawalNotifications",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
