using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V52 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumShareAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                newName: "MinimumShareAccountBalanceForTheRequestAmount");

            migrationBuilder.RenameColumn(
                name: "MaximumShareAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                newName: "MinimumInspectionFeeRate");

            migrationBuilder.RenameColumn(
                name: "ShareAccountCoverageRate",
                table: "LoanApplications",
                newName: "ShareAccountCoverageAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultInspectionFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumInspectionFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumShareAccountBalanceForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InspectionFee",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OtherFeeName",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultInspectionFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumInspectionFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumShareAccountBalanceForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "InspectionFee",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "OtherFeeName",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "MinimumShareAccountBalanceForTheRequestAmount",
                table: "LoanProducts",
                newName: "MinimumShareAccountBalanceRateForTheRequestAmount");

            migrationBuilder.RenameColumn(
                name: "MinimumInspectionFeeRate",
                table: "LoanProducts",
                newName: "MaximumShareAccountBalanceRateForTheRequestAmount");

            migrationBuilder.RenameColumn(
                name: "ShareAccountCoverageAmount",
                table: "LoanApplications",
                newName: "ShareAccountCoverageRate");
        }
    }
}
