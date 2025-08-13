using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V79 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingProductFees_FeeDto_FeeId",
                table: "SavingProductFees");

            migrationBuilder.DropTable(
                name: "WithdrawalFeeDto");

            migrationBuilder.DropTable(
                name: "FeePolicyDto");

            migrationBuilder.DropTable(
                name: "FeeDto");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingProductFees_Fees_FeeId",
                table: "SavingProductFees",
                column: "FeeId",
                principalTable: "Fees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingProductFees_Fees_FeeId",
                table: "SavingProductFees");

            migrationBuilder.CreateTable(
                name: "FeeDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAppliesOnHoliday = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeePolicyDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "WithdrawalFeeDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeePolicyID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WithdrawalParameterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_FeePolicyDto_FeeId",
                table: "FeePolicyDto",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalFeeDto_FeePolicyID",
                table: "WithdrawalFeeDto",
                column: "FeePolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalFeeDto_WithdrawalParameterId",
                table: "WithdrawalFeeDto",
                column: "WithdrawalParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingProductFees_FeeDto_FeeId",
                table: "SavingProductFees",
                column: "FeeId",
                principalTable: "FeeDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
