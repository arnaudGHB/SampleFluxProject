using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepositLimits");

            migrationBuilder.DropTable(
                name: "TransferLimits");

            migrationBuilder.DropTable(
                name: "WithdrawalLimits");

            migrationBuilder.DropColumn(
                name: "AccountOwnershipType",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "AlertBalance",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CanExpire",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ClosingFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "EntryFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "InterestCalculationFrequency",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ManagementFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ManagementFeeFrequency",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ReopeningFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "TermEndDate",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "TermStartDate",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "YearlyInterestRate",
                table: "SavingProducts");

            migrationBuilder.RenameColumn(
                name: "IsTerm",
                table: "SavingProducts",
                newName: "IsTermProduct");

            migrationBuilder.RenameColumn(
                name: "IsOverdraftAllowed",
                table: "SavingProducts",
                newName: "IsCapitalizeInterest");

            migrationBuilder.RenameColumn(
                name: "IsOperations",
                table: "SavingProducts",
                newName: "ActiveStatus");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "AccountingEvents",
                newName: "OperationType");

            migrationBuilder.AddColumn<string>(
                name: "chartOfAccountId",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDebit",
                table: "Tellers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "operationEventAttributeId",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostingFrequency",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
                name: "CurrencyId",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InterestAccrualFrequency",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TellerInterestBalance",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OperationAccountTypeId",
                table: "AccountingEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CashDepositParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepositType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_CashDepositParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashDepositParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloseFeeParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CloseFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CloseFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CloseFeeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloseFeeParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryFeeParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntryFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_EntryFeeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryFeeParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagementFeeParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ManagementFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManagementFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManagementFeeFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_ManagementFeeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementFeeParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReopenFeeParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReopenFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReopenFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ReopenFeeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReopenFeeParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermDepositParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DurationInDay = table.Column<int>(type: "int", nullable: false),
                    DurationInPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EarlyCloseFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EarlyCloseFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountIdPrincipalAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartOfAccountIdInterestAccrualAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartOfAccountIdInterestExpenseAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartOfAccountIdInterestWriteOffAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartOfAccountIdEarlyCloseFeeAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_TermDepositParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermDepositParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransferType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_TransferParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WithdrawalType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WithdrawalFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WithdrawalFeeFlat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationEventAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_WithdrawalParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalParameters_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashDepositParameters_ProductId",
                table: "CashDepositParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CloseFeeParameters_ProductId",
                table: "CloseFeeParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryFeeParameters_ProductId",
                table: "EntryFeeParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementFeeParameters_ProductId",
                table: "ManagementFeeParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReopenFeeParameters_ProductId",
                table: "ReopenFeeParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TermDepositParameters_ProductId",
                table: "TermDepositParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferParameters_ProductId",
                table: "TransferParameters",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalParameters_ProductId",
                table: "WithdrawalParameters",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashDepositParameters");

            migrationBuilder.DropTable(
                name: "CloseFeeParameters");

            migrationBuilder.DropTable(
                name: "EntryFeeParameters");

            migrationBuilder.DropTable(
                name: "ManagementFeeParameters");

            migrationBuilder.DropTable(
                name: "ReopenFeeParameters");

            migrationBuilder.DropTable(
                name: "TermDepositParameters");

            migrationBuilder.DropTable(
                name: "TransferParameters");

            migrationBuilder.DropTable(
                name: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "chartOfAccountId",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "isDebit",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "operationEventAttributeId",
                table: "Tellers");

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
                name: "CurrencyId",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "InterestAccrualFrequency",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "TellerInterestBalance",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "OperationAccountTypeId",
                table: "AccountingEvents");

            migrationBuilder.RenameColumn(
                name: "IsTermProduct",
                table: "SavingProducts",
                newName: "IsTerm");

            migrationBuilder.RenameColumn(
                name: "IsCapitalizeInterest",
                table: "SavingProducts",
                newName: "IsOverdraftAllowed");

            migrationBuilder.RenameColumn(
                name: "ActiveStatus",
                table: "SavingProducts",
                newName: "IsOperations");

            migrationBuilder.RenameColumn(
                name: "OperationType",
                table: "AccountingEvents",
                newName: "ProductID");

            migrationBuilder.AlterColumn<string>(
                name: "PostingFrequency",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AccountOwnershipType",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AlertBalance",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CanExpire",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EntryFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InterestCalculationFrequency",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ManagementFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ManagementFeeFrequency",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReopeningFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermEndDate",
                table: "SavingProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermStartDate",
                table: "SavingProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "YearlyInterestRate",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "DepositLimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepositType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepositLimits_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferLimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferLimits_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalLimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WithdrawalType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalLimits_SavingProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepositLimits_ProductId",
                table: "DepositLimits",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferLimits_ProductId",
                table: "TransferLimits",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalLimits_ProductId",
                table: "WithdrawalLimits",
                column: "ProductId");
        }
    }
}
