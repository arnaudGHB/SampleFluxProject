using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaidFeeBeforeProcessing",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidAllFeeUpFront",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidFeeAfterProcessing",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidFeeBeforeProcessing",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCashDeskPayment",
                table: "LoanApplicationFee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaidFeeBeforeProcessing",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsPaidAllFeeUpFront",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsPaidFeeAfterProcessing",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsPaidFeeBeforeProcessing",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsCashDeskPayment",
                table: "LoanApplicationFee");
        }
    }
}
