using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Co_Obligor",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Co_OperationGurantor",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Deposit",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OShares",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherGuaranteeFund",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PShares",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageOfCollateralCoverage",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageOfLiquidityCoverage",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageOfOverAllCoverage",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Savings",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Shortee",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFundGuranteed",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Co_Obligor",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Co_OperationGurantor",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "OShares",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "OtherGuaranteeFund",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PShares",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PercentageOfCollateralCoverage",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PercentageOfLiquidityCoverage",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PercentageOfOverAllCoverage",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Savings",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Shortee",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "TotalFundGuranteed",
                table: "Loans");
        }
    }
}
