using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GracePeriod",
                table: "LoanApplications",
                newName: "GracePeriodBeforeFirstPayment");

            migrationBuilder.AddColumn<int>(
                name: "GracePeriodAfterMaturityDate",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GracePeriodAfterMaturityDate",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "GracePeriodBeforeFirstPayment",
                table: "LoanApplications",
                newName: "GracePeriod");
        }
    }
}
