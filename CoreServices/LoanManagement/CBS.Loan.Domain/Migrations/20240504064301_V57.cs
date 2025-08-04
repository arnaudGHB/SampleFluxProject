using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaulChargesStartDayAfterLoanDueDate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultChargeToAppliedPercentage",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumChargesStartDayAfterLoanDueDate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumChargesStartDayAfterLoanDueDate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaulChargesStartDayAfterLoanDueDate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultChargeToAppliedPercentage",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumChargesStartDayAfterLoanDueDate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumChargesStartDayAfterLoanDueDate",
                table: "LoanProducts");
        }
    }
}
