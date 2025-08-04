using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxDue",
                table: "LoanAmortizations");

            migrationBuilder.AddColumn<decimal>(
                name: "LastCalculatedInterest",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "PreviousInstallmentDue",
                table: "LoanAmortizations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCalculatedInterest",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PreviousInstallmentDue",
                table: "LoanAmortizations");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxDue",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
