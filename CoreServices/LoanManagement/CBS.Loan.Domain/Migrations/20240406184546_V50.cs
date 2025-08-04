using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForEarlyPartialRepaymentFeeIncome",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForEarlyTotalRepaymentFeeIncome",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForInterestIncome",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForLoanLossReserve",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForLoanLossReserveInterest",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForLoanLossReservePenalties",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionOnInterest",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionOnLateFees",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionReversalOnInterest",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionReversalOnLateFees",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionReversalOnPrincipal",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForWriteOffInterest",
                table: "LoanProducts");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForWriteOffPotfolio",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForWriteOffPrincipal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForWriteOffPrincipal",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForWriteOffPotfolio");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForEarlyPartialRepaymentFeeIncome",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForEarlyTotalRepaymentFeeIncome",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForInterestIncome",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForLoanLossReserve",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForLoanLossReserveInterest",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForLoanLossReservePenalties",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionOnInterest",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionOnLateFees",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionReversalOnInterest",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionReversalOnLateFees",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionReversalOnPrincipal",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForWriteOffInterest",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
