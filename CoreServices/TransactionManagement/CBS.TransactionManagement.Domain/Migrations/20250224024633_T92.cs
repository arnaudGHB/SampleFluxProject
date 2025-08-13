using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T92 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnldLoan",
                table: "SalaryExtract",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductId",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductName",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnldLoan",
                table: "SalaryAnalysisResultDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductId",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductName",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnldLoan",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "LoanProductId",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "LoanProductName",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "IsOnldLoan",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.DropColumn(
                name: "LoanProductId",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.DropColumn(
                name: "LoanProductName",
                table: "SalaryAnalysisResultDetails");
        }
    }
}
