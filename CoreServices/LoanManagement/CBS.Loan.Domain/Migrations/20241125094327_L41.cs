using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumDurationPeriod",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "MinimumDurationPeriod",
                table: "LoanProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumDurationPeriod",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumDurationPeriod",
                table: "LoanProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
