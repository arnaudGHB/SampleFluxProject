using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterestType",
                table: "LoanApplications",
                newName: "AmortizationType");

            migrationBuilder.AddColumn<bool>(
                name: "IsVat",
                table: "Taxes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVat",
                table: "Taxes");

            migrationBuilder.RenameColumn(
                name: "AmortizationType",
                table: "LoanApplications",
                newName: "InterestType");
        }
    }
}
