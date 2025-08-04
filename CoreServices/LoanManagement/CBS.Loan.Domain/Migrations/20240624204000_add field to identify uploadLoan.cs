using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addfieldtoidentifyuploadLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUpload",
                table: "Loans",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUpload",
                table: "LoanApplications",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsUpload",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsUpload",
                table: "LoanApplications");
        }
    }
}
