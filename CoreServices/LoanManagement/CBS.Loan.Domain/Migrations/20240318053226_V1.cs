using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumCreditInsurancePercentage",
                table: "LoanProducts",
                newName: "MinimumShareAccountBalanceRateForTheRequestAmount");

            migrationBuilder.RenameColumn(
                name: "MaximumCreditInsurancePercentage",
                table: "LoanProducts",
                newName: "MinimumSavingAccountBalanceRateForTheRequestAmount");

            migrationBuilder.RenameColumn(
                name: "DefaultCreditInsurancePercentage",
                table: "LoanProducts",
                newName: "MinimumSalaryAccountBalanceRateForTheRequestAmount");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeeDeductedUpFront",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterestDeductedUpFront",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredCollateral",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredSalaryccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredSavingAccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredShareAccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequredIrrivocableSalaryTransfer",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequresRegisteredPublicAuthority",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumMaximumSalaryAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumSavingAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumShareAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsFeeDeductedUpFront",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsInterestDeductedUpFront",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequiredCollateral",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequiredSalaryccount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequiredSavingAccount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequiredShareAccount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequredIrrivocableSalaryTransfer",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsRequresRegisteredPublicAuthority",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumMaximumSalaryAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumSavingAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumShareAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.RenameColumn(
                name: "MinimumShareAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                newName: "MinimumCreditInsurancePercentage");

            migrationBuilder.RenameColumn(
                name: "MinimumSavingAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                newName: "MaximumCreditInsurancePercentage");

            migrationBuilder.RenameColumn(
                name: "MinimumSalaryAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                newName: "DefaultCreditInsurancePercentage");
        }
    }
}
