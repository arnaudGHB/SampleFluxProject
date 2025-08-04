using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionFee",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "OtherFee",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "OtherFeeName",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "ProcessingFee",
                table: "LoanApplications");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPayment",
                table: "LoanApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsIninitalProcessingFeePaid",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPayment",
                table: "LoanApplicationFee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaidBy",
                table: "LoanApplicationFee",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfPayment",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsIninitalProcessingFeePaid",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DateOfPayment",
                table: "LoanApplicationFee");

            migrationBuilder.DropColumn(
                name: "PaidBy",
                table: "LoanApplicationFee");

            migrationBuilder.AddColumn<decimal>(
                name: "InspectionFee",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherFee",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OtherFeeName",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProcessingFee",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
