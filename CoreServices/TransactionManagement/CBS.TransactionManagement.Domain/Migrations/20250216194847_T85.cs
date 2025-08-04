using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T85 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembersName",
                table: "SalaryExtract",
                newName: "SalaryAnalysisResultId");

            migrationBuilder.RenameColumn(
                name: "LoanPrincipal",
                table: "SalaryExtract",
                newName: "VAT");

            migrationBuilder.RenameColumn(
                name: "LoanAmount",
                table: "SalaryExtract",
                newName: "TotalLoanRepayment");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "SalaryExtract",
                newName: "ExtrationDate");

            migrationBuilder.AlterColumn<string>(
                name: "BranchName",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ExecutedBy",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "SalaryExtract",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "LoanCapital",
                table: "SalaryExtract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Matricule",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemberName",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PreferenceShares",
                table: "SalaryExtract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingSalary",
                table: "SalaryExtract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "SalaryExtract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutedBy",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "LoanCapital",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "MemberName",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "PreferenceShares",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "RemainingSalary",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "SalaryExtract");

            migrationBuilder.RenameColumn(
                name: "VAT",
                table: "SalaryExtract",
                newName: "LoanPrincipal");

            migrationBuilder.RenameColumn(
                name: "TotalLoanRepayment",
                table: "SalaryExtract",
                newName: "LoanAmount");

            migrationBuilder.RenameColumn(
                name: "SalaryAnalysisResultId",
                table: "SalaryExtract",
                newName: "MembersName");

            migrationBuilder.RenameColumn(
                name: "ExtrationDate",
                table: "SalaryExtract",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "BranchName",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
