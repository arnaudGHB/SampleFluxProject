using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V76 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "WithdrawalFeeFlat",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "WithdrawalFeeRate",
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
                name: "DepositFeeRate",
                table: "CashDepositParameters");

            migrationBuilder.DropColumn(
                name: "DepositFormFee",
                table: "CashDepositParameters");

            migrationBuilder.AddColumn<bool>(
                name: "FridayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MWednessdayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MondayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SaturdayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SundayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThursdayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TuesDayIsHoliday",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FeeDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAppliesOnHoliday = table.Column<bool>(type: "bit", nullable: false),
                    FeeType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAppliesOnHoliday = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Fees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeePolicyDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeePolicyDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeePolicyDto_FeeDto_FeeId",
                        column: x => x.FeeId,
                        principalTable: "FeeDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavingProductFees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavingProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_SavingProductFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingProductFees_FeeDto_FeeId",
                        column: x => x.FeeId,
                        principalTable: "FeeDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavingProductFees_SavingProducts_SavingProductId",
                        column: x => x.SavingProductId,
                        principalTable: "SavingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeePolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_FeePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeePolicies_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalFeeDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeePolicyID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WithdrawalParameterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_WithdrawalFeeDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalFeeDto_FeePolicyDto_FeePolicyID",
                        column: x => x.FeePolicyID,
                        principalTable: "FeePolicyDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WithdrawalFeeDto_WithdrawalParameters_WithdrawalParameterId",
                        column: x => x.WithdrawalParameterId,
                        principalTable: "WithdrawalParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeePolicies_FeeId",
                table: "FeePolicies",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeePolicyDto_FeeId",
                table: "FeePolicyDto",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingProductFees_FeeId",
                table: "SavingProductFees",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingProductFees_SavingProductId",
                table: "SavingProductFees",
                column: "SavingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalFeeDto_FeePolicyID",
                table: "WithdrawalFeeDto",
                column: "FeePolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalFeeDto_WithdrawalParameterId",
                table: "WithdrawalFeeDto",
                column: "WithdrawalParameterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeePolicies");

            migrationBuilder.DropTable(
                name: "SavingProductFees");

            migrationBuilder.DropTable(
                name: "WithdrawalFeeDto");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "FeePolicyDto");

            migrationBuilder.DropTable(
                name: "FeeDto");

            migrationBuilder.DropColumn(
                name: "FridayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "MWednessdayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "MondayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "SaturdayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "SundayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "ThursdayIsHoliday",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "TuesDayIsHoliday",
                table: "Configs");

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
                name: "WithdrawalFeeFlat",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawalFeeRate",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.AddColumn<decimal>(
                name: "DepositFeeRate",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositFormFee",
                table: "CashDepositParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
