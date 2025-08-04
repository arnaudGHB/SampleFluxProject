using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AffectScoring",
                table: "LoanDeliquencyConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyFine",
                table: "LoanDeliquencyConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReportToCreditOffice",
                table: "LoanDeliquencyConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffectScoring",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "ApplyFine",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "ReportToCreditOffice",
                table: "LoanDeliquencyConfigurations");
        }
    }
}
