using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V58 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChargesPercentage",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestWaiverPercentage",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsChargesApplied",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterestWaiverApplied",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDaysToApplyCharges",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargesPercentage",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "InterestWaiverPercentage",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsChargesApplied",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsInterestWaiverApplied",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "NumberOfDaysToApplyCharges",
                table: "LoanApplications");
        }
    }
}
