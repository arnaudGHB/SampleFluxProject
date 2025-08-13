using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V84 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFee",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AccountTypeForYearyDeductionOfBuildingContribution",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalForm",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "YearBuildingContributionFee",
                table: "MemberAccountActivationPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "TotalFee",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "AccountTypeForYearyDeductionOfBuildingContribution",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "LegalForm",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "YearBuildingContributionFee",
                table: "MemberAccountActivationPolicies");
        }
    }
}
