using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LoanId",
                table: "DailyInterestCalculations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "CalculatedVat",
                table: "DailyInterestCalculations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "DailyInterestCalculations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_DailyInterestCalculations_LoanId",
                table: "DailyInterestCalculations",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyInterestCalculations_Loans_LoanId",
                table: "DailyInterestCalculations",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyInterestCalculations_Loans_LoanId",
                table: "DailyInterestCalculations");

            migrationBuilder.DropIndex(
                name: "IX_DailyInterestCalculations_LoanId",
                table: "DailyInterestCalculations");

            migrationBuilder.DropColumn(
                name: "CalculatedVat",
                table: "DailyInterestCalculations");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "DailyInterestCalculations");

            migrationBuilder.AlterColumn<string>(
                name: "LoanId",
                table: "DailyInterestCalculations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
