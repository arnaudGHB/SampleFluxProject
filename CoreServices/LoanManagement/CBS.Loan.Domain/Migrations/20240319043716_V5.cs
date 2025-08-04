using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCollateralProvided",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "IsGuarantorProvided",
                table: "LoanApplications",
                newName: "IsThereCollateral");

            migrationBuilder.RenameColumn(
                name: "IsGuaranteeProvided",
                table: "LoanApplications",
                newName: "IsDisbursted");

            migrationBuilder.RenameColumn(
                name: "InsuranceCoverageRate",
                table: "LoanApplications",
                newName: "SalaryAccountCoverageRate");

            migrationBuilder.RenameColumn(
                name: "GuaratorAccountCoverageRate",
                table: "LoanApplications",
                newName: "GuaratorSavingAccountCoverageRate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalaryAccountCoverageRate",
                table: "LoanApplications",
                newName: "InsuranceCoverageRate");

            migrationBuilder.RenameColumn(
                name: "IsThereCollateral",
                table: "LoanApplications",
                newName: "IsGuarantorProvided");

            migrationBuilder.RenameColumn(
                name: "IsDisbursted",
                table: "LoanApplications",
                newName: "IsGuaranteeProvided");

            migrationBuilder.RenameColumn(
                name: "GuaratorSavingAccountCoverageRate",
                table: "LoanApplications",
                newName: "GuaratorAccountCoverageRate");

            migrationBuilder.AddColumn<bool>(
                name: "IsCollateralProvided",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
