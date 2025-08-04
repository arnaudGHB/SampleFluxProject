using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "LoanCollaterals",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "LoanCollaterals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LoanCollaterals");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "LoanCollaterals",
                newName: "Rate");
        }
    }
}
