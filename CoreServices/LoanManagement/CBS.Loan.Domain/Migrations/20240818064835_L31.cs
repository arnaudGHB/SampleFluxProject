using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRestructured",
                table: "Loans",
                newName: "StopInterestCalculation");

            migrationBuilder.RenameColumn(
                name: "IsRescheduled",
                table: "LoanAmortizations",
                newName: "IsStructured");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInterestWastStoped",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoanStructuringDate",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LoanStructuringStatus",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoppedBy",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInterestCalaculationWasStoped",
                table: "LoanApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestructuredBalance",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "StopInterestCalculation",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StopedBy",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanStructuringStatus",
                table: "LoanAmortizations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateInterestWastStoped",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanStructuringDate",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanStructuringStatus",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "StoppedBy",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DateInterestCalaculationWasStoped",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "RestructuredBalance",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "StopInterestCalculation",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "StopedBy",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "LoanStructuringStatus",
                table: "LoanAmortizations");

            migrationBuilder.RenameColumn(
                name: "StopInterestCalculation",
                table: "Loans",
                newName: "IsRestructured");

            migrationBuilder.RenameColumn(
                name: "IsStructured",
                table: "LoanAmortizations",
                newName: "IsRescheduled");
        }
    }
}
