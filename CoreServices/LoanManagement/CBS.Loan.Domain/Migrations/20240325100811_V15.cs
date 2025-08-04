using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccrualInterestBalance",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "FeeBalance",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PenaltyBalance",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "RepaymentType",
                table: "Refunds",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "PaymentMode",
                table: "Refunds",
                newName: "PaymentChannel");

            migrationBuilder.RenameColumn(
                name: "Paid",
                table: "Refunds",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "TaxBalance",
                table: "Loans",
                newName: "LoanAmount");

            migrationBuilder.RenameColumn(
                name: "PrincipalBalance",
                table: "Loans",
                newName: "Fee");

            migrationBuilder.RenameColumn(
                name: "PrincipalDue",
                table: "LoanAmortizations",
                newName: "TaxPaid");

            migrationBuilder.RenameColumn(
                name: "PrincipalBalance",
                table: "LoanAmortizations",
                newName: "TaxDue");

            migrationBuilder.RenameColumn(
                name: "PendingDue",
                table: "LoanAmortizations",
                newName: "PrincipalPaid");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Refunds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEventData",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFeePaidUpFront",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterestRunning",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestPaid",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PenaltyPaid",
                table: "LoanAmortizations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "LastEventData",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsFeePaidUpFront",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsInterestRunning",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "InterestPaid",
                table: "LoanAmortizations");

            migrationBuilder.DropColumn(
                name: "PenaltyPaid",
                table: "LoanAmortizations");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Refunds",
                newName: "RepaymentType");

            migrationBuilder.RenameColumn(
                name: "PaymentChannel",
                table: "Refunds",
                newName: "PaymentMode");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Refunds",
                newName: "Paid");

            migrationBuilder.RenameColumn(
                name: "LoanAmount",
                table: "Loans",
                newName: "TaxBalance");

            migrationBuilder.RenameColumn(
                name: "Fee",
                table: "Loans",
                newName: "PrincipalBalance");

            migrationBuilder.RenameColumn(
                name: "TaxPaid",
                table: "LoanAmortizations",
                newName: "PrincipalDue");

            migrationBuilder.RenameColumn(
                name: "TaxDue",
                table: "LoanAmortizations",
                newName: "PrincipalBalance");

            migrationBuilder.RenameColumn(
                name: "PrincipalPaid",
                table: "LoanAmortizations",
                newName: "PendingDue");

            migrationBuilder.AddColumn<decimal>(
                name: "AccrualInterestBalance",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeBalance",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PenaltyBalance",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
