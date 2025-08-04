using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanCycles",
                table: "LoanCycles");

            migrationBuilder.RenameTable(
                name: "LoanCycles",
                newName: "LoanProductCategories");

            migrationBuilder.AddColumn<string>(
                name: "DeliquentStatus",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDeliquecyProcessedDate",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductCategoryId",
                table: "LoanProducts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanProductCategories",
                table: "LoanProductCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_LoanProductCategoryId",
                table: "LoanProducts",
                column: "LoanProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_LoanProductCategories_LoanProductCategoryId",
                table: "LoanProducts",
                column: "LoanProductCategoryId",
                principalTable: "LoanProductCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_LoanProductCategories_LoanProductCategoryId",
                table: "LoanProducts");

            migrationBuilder.DropIndex(
                name: "IX_LoanProducts_LoanProductCategoryId",
                table: "LoanProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanProductCategories",
                table: "LoanProductCategories");

            migrationBuilder.DropColumn(
                name: "DeliquentStatus",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LastDeliquecyProcessedDate",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanProductCategoryId",
                table: "LoanProducts");

            migrationBuilder.RenameTable(
                name: "LoanProductCategories",
                newName: "LoanCycles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanCycles",
                table: "LoanCycles",
                column: "Id");
        }
    }
}
