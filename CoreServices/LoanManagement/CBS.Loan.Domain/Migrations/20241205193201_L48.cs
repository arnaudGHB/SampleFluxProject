using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForProvisionOnPrincipal",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForProvisionMoreThanTwoYear");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForFee",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForProvisionMoreThanThreeYear");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForAccrualInterest",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForProvisionMoreThanOneYear");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForInterestReceived",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionMoreThanFourYear",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForInterestReceived",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionMoreThanFourYear",
                table: "LoanProducts");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForProvisionMoreThanTwoYear",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForProvisionOnPrincipal");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForProvisionMoreThanThreeYear",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForFee");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdForProvisionMoreThanOneYear",
                table: "LoanProducts",
                newName: "ChartOfAccountIdForAccrualInterest");
        }
    }
}
