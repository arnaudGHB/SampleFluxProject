using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanProductFeeJoins");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "MaximumAmount",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "MinimumAmount",
                table: "Fees");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "Fees",
                newName: "FeeBase");

            migrationBuilder.AddColumn<string>(
                name: "AccountingEventCode",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FeeRanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentageValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_FeeRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeRanges_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplicationFee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeRangeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_LoanApplicationFee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApplicationFee_FeeRanges_FeeRangeId",
                        column: x => x.FeeRangeId,
                        principalTable: "FeeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanApplicationFee_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeeRanges_FeeId",
                table: "FeeRanges",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationFee_FeeRangeId",
                table: "LoanApplicationFee",
                column: "FeeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationFee_LoanApplicationId",
                table: "LoanApplicationFee",
                column: "LoanApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanApplicationFee");

            migrationBuilder.DropTable(
                name: "FeeRanges");

            migrationBuilder.DropColumn(
                name: "AccountingEventCode",
                table: "Fees");

            migrationBuilder.RenameColumn(
                name: "FeeBase",
                table: "Fees",
                newName: "BranchId");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumAmount",
                table: "Fees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumAmount",
                table: "Fees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "LoanProductFeeJoins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProductFeeJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductFeeJoins_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanProductFeeJoins_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductFeeJoins_FeeId",
                table: "LoanProductFeeJoins",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductFeeJoins_LoanProductId",
                table: "LoanProductFeeJoins",
                column: "LoanProductId");
        }
    }
}
