using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileUploadId",
                table: "SalaryExtract",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingDate",
                table: "SalaryExtract",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "SalaryExtract",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SalaryExtractDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MembersName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saving = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shares = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUploadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_SalaryExtract_FileUploadId",
                table: "SalaryExtract",
                column: "FileUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryExtractDto_FileUploadId",
                table: "SalaryExtractDto",
                column: "FileUploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryExtract_FileUpload_FileUploadId",
                table: "SalaryExtract",
                column: "FileUploadId",
                principalTable: "FileUpload",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryExtract_FileUpload_FileUploadId",
                table: "SalaryExtract");

            migrationBuilder.DropTable(
                name: "SalaryExtractDto");

            migrationBuilder.DropIndex(
                name: "IX_SalaryExtract_FileUploadId",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "AccountingDate",
                table: "SalaryExtract");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SalaryExtract");

            migrationBuilder.AlterColumn<string>(
                name: "FileUploadId",
                table: "SalaryExtract",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
