using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class LL1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProductRepaymentOrder_LoanProducts_LoanProductId",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropIndex(
                name: "IX_LoanProductRepaymentOrder_LoanProductId",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "LoanDeliquencyPeriod",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "LoanProductId",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "LoanApplications");

            migrationBuilder.AddColumn<string>(
                name: "LoanDeliquencyPeriod",
                table: "LoanProductRepaymentOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoanProductId",
                table: "LoanProductRepaymentOrder",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductRepaymentOrder_LoanProductId",
                table: "LoanProductRepaymentOrder",
                column: "LoanProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProductRepaymentOrder_LoanProducts_LoanProductId",
                table: "LoanProductRepaymentOrder",
                column: "LoanProductId",
                principalTable: "LoanProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
