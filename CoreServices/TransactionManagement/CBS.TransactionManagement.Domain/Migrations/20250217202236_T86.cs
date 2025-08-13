using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T86 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUploadIdReferenceId",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanId",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanType",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StandingOrderAmount",
                table: "SalaryAnalysisResultDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StandingOrderStatement",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUploadIdReferenceId",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.DropColumn(
                name: "LoanType",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.DropColumn(
                name: "StandingOrderAmount",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.DropColumn(
                name: "StandingOrderStatement",
                table: "SalaryAnalysisResultDetails");
        }
    }
}
