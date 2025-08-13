using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T67 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionMoreThanFourYear",
                table: "OldLoanAccountingMapings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionMoreThanOneYear",
                table: "OldLoanAccountingMapings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionMoreThanThreeYear",
                table: "OldLoanAccountingMapings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdForProvisionMoreThanTwoYear",
                table: "OldLoanAccountingMapings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalaryAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalMembers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLoanCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLoanInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLoanRepayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSavings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalShares = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRemainingSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBranches = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUploadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryAnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryAnalysisResults_FileUpload_FileUploadId",
                        column: x => x.FileUploadId,
                        principalTable: "FileUpload",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryUploadModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SalaryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaisonBankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUploadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryUploadModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryUploadModels_FileUpload_FileUploadId",
                        column: x => x.FileUploadId,
                        principalTable: "FileUpload",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandingOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceAccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryAnalysisResultDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLoanRepayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Savings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shares = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalaryAnalysisResultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryAnalysisResultDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryAnalysisResultDetails_SalaryAnalysisResults_SalaryAnalysisResultId",
                        column: x => x.SalaryAnalysisResultId,
                        principalTable: "SalaryAnalysisResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAnalysisResultDetails_SalaryAnalysisResultId",
                table: "SalaryAnalysisResultDetails",
                column: "SalaryAnalysisResultId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAnalysisResults_FileUploadId",
                table: "SalaryAnalysisResults",
                column: "FileUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryUploadModels_FileUploadId",
                table: "SalaryUploadModels",
                column: "FileUploadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryAnalysisResultDetails");

            migrationBuilder.DropTable(
                name: "SalaryUploadModels");

            migrationBuilder.DropTable(
                name: "StandingOrders");

            migrationBuilder.DropTable(
                name: "SalaryAnalysisResults");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionMoreThanFourYear",
                table: "OldLoanAccountingMapings");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionMoreThanOneYear",
                table: "OldLoanAccountingMapings");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionMoreThanThreeYear",
                table: "OldLoanAccountingMapings");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdForProvisionMoreThanTwoYear",
                table: "OldLoanAccountingMapings");
        }
    }
}
