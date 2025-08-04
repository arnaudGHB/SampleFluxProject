using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Co_obligorMustHaveFundToGuranteeLoan",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPercentageCoverageOfShortee",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPercentageRefundBeforeRefinancing",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDaysToStopInterestCalculation",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShorteeMustHaveFundToGuranteeLoan",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StopInterestCalculationAtLoanMaturityDate",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GuarantorType",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Co_obligorMustHaveFundToGuranteeLoan",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumPercentageCoverageOfShortee",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumPercentageRefundBeforeRefinancing",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "NumberOfDaysToStopInterestCalculation",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ShorteeMustHaveFundToGuranteeLoan",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "StopInterestCalculationAtLoanMaturityDate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "GuarantorType",
                table: "LoanGuarantors");
        }
    }
}
