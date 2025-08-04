using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DeliquentInterest",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestBalance",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestDue",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrincipalBalance",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliquentInterest",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "InterestBalance",
                table: "LoanAmortizations");

            migrationBuilder.DropColumn(
                name: "InterestDue",
                table: "LoanAmortizations");

            migrationBuilder.DropColumn(
                name: "PrincipalBalance",
                table: "LoanAmortizations");
        }
    }
}
