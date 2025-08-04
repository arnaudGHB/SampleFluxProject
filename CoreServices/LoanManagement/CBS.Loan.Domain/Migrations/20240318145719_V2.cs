using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BlockedGuarantorAccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockedSalaryAccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockedSavingAccount",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedGuarantorAccount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "BlockedSalaryAccount",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "BlockedSavingAccount",
                table: "LoanProducts");
        }
    }
}
