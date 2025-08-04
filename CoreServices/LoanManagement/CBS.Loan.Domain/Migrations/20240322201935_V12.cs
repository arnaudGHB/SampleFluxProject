using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Documents",
                newName: "DocumentType");

            migrationBuilder.AddColumn<string>(
                name: "DocumentId",
                table: "LoanApplications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_DocumentId",
                table: "LoanApplications",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_Documents_DocumentId",
                table: "LoanApplications",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_Documents_DocumentId",
                table: "LoanApplications");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplications_DocumentId",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "Documents",
                newName: "Type");
        }
    }
}
