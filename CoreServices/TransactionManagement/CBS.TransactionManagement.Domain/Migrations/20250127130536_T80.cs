using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryExtractDto");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "SalaryUploadModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "SalaryAnalysisResultDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "SalaryUploadModels");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "SalaryAnalysisResultDetails");

            migrationBuilder.CreateTable(
                name: "SalaryExtractDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FileUploadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MembersName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saving = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shares = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryExtractDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryExtractDto_FileUpload_FileUploadId",
                        column: x => x.FileUploadId,
                        principalTable: "FileUpload",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryExtractDto_FileUploadId",
                table: "SalaryExtractDto",
                column: "FileUploadId");
        }
    }
}
