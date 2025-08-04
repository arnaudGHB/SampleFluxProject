using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChargesAreAppliedToInterestOrBalance",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChargesStopAfterHowManyDaysFromStart",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsChargesApplied",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterestWaiverApplied",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumChargesToAppliedPercentage",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumInterestWaiver",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumChargesToAppliedInPercentage",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumInterestWaiver",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargesAreAppliedToInterestOrBalance",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "ChargesStopAfterHowManyDaysFromStart",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsChargesApplied",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsInterestWaiverApplied",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumChargesToAppliedPercentage",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumInterestWaiver",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumChargesToAppliedInPercentage",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumInterestWaiver",
                table: "LoanProducts");
        }
    }
}
