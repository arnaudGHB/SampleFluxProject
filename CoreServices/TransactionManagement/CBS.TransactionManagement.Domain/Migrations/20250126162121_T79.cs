using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T79 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvalaibleForExecution",
                table: "FileUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalBranchesInvolvedInPayrolProcessing",
                table: "FileUpload",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalBranchesThatHaveExecutedPayrol",
                table: "FileUpload",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvalaibleForExecution",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "TotalBranchesInvolvedInPayrolProcessing",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "TotalBranchesThatHaveExecutedPayrol",
                table: "FileUpload");
        }
    }
}
