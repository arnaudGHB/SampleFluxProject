using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts");

            migrationBuilder.DropIndex(
                name: "IX_LoanProducts_TaxId",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "CalculateInterestOnEachRepaymentOnProRatabase",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultCollateralPercentage",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultDurationsPeriod",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultInspectionFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultInterestRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultLoanAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultNumberOfRepayment",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DefaultProcessingFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "EarlyPartialRepaymentFee",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "EarlyTotalRepaymentFee",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "FirstRepaymentAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "HowShoudInterestBeCahrgedInLoanSchedule",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "HowShoudPrincipalBeCahrgedInLoanSchedule",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsEarlyPartialRepaymentFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsEarlyTotalRepaymentFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsFeeDeductedUpFront",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "IsInterestDeductedUpFront",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "LoanInterestType",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "LoanMaximumAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "LoanScheduleDescription",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "LoanTermDto",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaxTopUpLoanAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumCollateralPercentage",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumInspectionFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumMaximumSalaryAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumNumberOfRepayment",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumProcessingFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumSavingAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MaximumShareAccountBalanceForTheRequestAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinTopUpLoanAmount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumInspectionFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumProcessingFeeRate",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "DateOfPayment",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "TopUpAmount",
                table: "LoanProducts",
                newName: "MinimumDownPaymentPercentage");

            migrationBuilder.RenameColumn(
                name: "RepaymentType",
                table: "LoanProductRepaymentOrder",
                newName: "RepaymentTypeName");

            migrationBuilder.RenameColumn(
                name: "RepaymentOrder",
                table: "LoanProductRepaymentOrder",
                newName: "InterestOrder");

            migrationBuilder.RenameColumn(
                name: "IsFeePaidUpFront",
                table: "LoanApplications",
                newName: "RequiredDownPaymentCoverageRate");

            migrationBuilder.AddColumn<string>(
                name: "LoanId",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CapitalOrder",
                table: "LoanProductRepaymentOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CapitalRate",
                table: "LoanProductRepaymentOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FineOrder",
                table: "LoanProductRepaymentOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FineRate",
                table: "LoanProductRepaymentOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "LoanProductRepaymentOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LoanDeliquencyPeriod",
                table: "LoanProductRepaymentOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyFeeToThisLoan",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyInterestToThisLoan",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAccountCoverageAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentCoverageAmountProvided",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDepositAccountCoverageAmount",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterestPaidUpFront",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreferenceShareAccountCoverageAmount",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTermDeposiAccountCoverageAmount",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PreferenceShareAccountCoverageAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryAccountCoverageAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TermDeposiAccountCoverageAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "CapitalOrder",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "CapitalRate",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "FineOrder",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "FineRate",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "LoanDeliquencyPeriod",
                table: "LoanProductRepaymentOrder");

            migrationBuilder.DropColumn(
                name: "ApplyFeeToThisLoan",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "ApplyInterestToThisLoan",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DepositAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DownPaymentCoverageAmountProvided",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsDepositAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsInterestPaidUpFront",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsPreferenceShareAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsTermDeposiAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "PreferenceShareAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "SalaryAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "TermDeposiAccountCoverageAmount",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "MinimumDownPaymentPercentage",
                table: "LoanProducts",
                newName: "TopUpAmount");

            migrationBuilder.RenameColumn(
                name: "RepaymentTypeName",
                table: "LoanProductRepaymentOrder",
                newName: "RepaymentType");

            migrationBuilder.RenameColumn(
                name: "InterestOrder",
                table: "LoanProductRepaymentOrder",
                newName: "RepaymentOrder");

            migrationBuilder.RenameColumn(
                name: "RequiredDownPaymentCoverageRate",
                table: "LoanApplications",
                newName: "IsFeePaidUpFront");

            migrationBuilder.AddColumn<bool>(
                name: "CalculateInterestOnEachRepaymentOnProRatabase",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultCollateralPercentage",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DefaultDurationsPeriod",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultInspectionFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultInterestRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultLoanAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DefaultNumberOfRepayment",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultProcessingFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EarlyPartialRepaymentFee",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EarlyTotalRepaymentFee",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FirstRepaymentAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "HowShoudInterestBeCahrgedInLoanSchedule",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HowShoudPrincipalBeCahrgedInLoanSchedule",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarlyPartialRepaymentFeeRate",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarlyTotalRepaymentFeeRate",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<string>(
                name: "LoanInterestType",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanMaximumAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LoanScheduleDescription",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanTermDto",
                table: "LoanProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxTopUpLoanAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumCollateralPercentage",
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
                name: "MaximumMaximumSalaryAccountBalanceRateForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaximumNumberOfRepayment",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumProcessingFeeRate",
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
                name: "MaximumShareAccountBalanceForTheRequestAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinTopUpLoanAmount",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumInspectionFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumProcessingFeeRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "LoanProducts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPayment",
                table: "LoanApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_TaxId",
                table: "LoanProducts",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
