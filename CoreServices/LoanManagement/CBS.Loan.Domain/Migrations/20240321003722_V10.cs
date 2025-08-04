using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                table: "LoanProducts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "LoanNotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendNotificationToMemberAfterLoanApproval = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationOTPToMemberForLoanApproval = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationOTPToBranchMsisdnForLoanApproval = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationToMemberAfterLoanApplication = table.Column<bool>(type: "bit", nullable: false),
                    BranchTelephoneForOTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchEmailForOTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendNotificationToMemberAfterLoanRejection = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationBySMS = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationByEmail = table.Column<bool>(type: "bit", nullable: false),
                    SendNotificationNoBothSMSAndEmail = table.Column<bool>(type: "bit", nullable: false),
                    ExprireTimeOfOTPInMinutes = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LoanNotificationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTPNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OPTCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitializedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_OTPNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTPNotifications_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OTPNotifications_LoanApplicationId",
                table: "OTPNotifications",
                column: "LoanApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts");

            migrationBuilder.DropTable(
                name: "LoanNotificationSettings");

            migrationBuilder.DropTable(
                name: "OTPNotifications");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "LoanApplications");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                table: "LoanProducts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_Taxes_TaxId",
                table: "LoanProducts",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id");
        }
    }
}
