using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "LoanApplicationFee",
                newName: "FeeAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "LoanApplicationFee",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "LoanApplicationFee");

            migrationBuilder.RenameColumn(
                name: "FeeAmount",
                table: "LoanApplicationFee",
                newName: "Amount");
        }
    }
}
