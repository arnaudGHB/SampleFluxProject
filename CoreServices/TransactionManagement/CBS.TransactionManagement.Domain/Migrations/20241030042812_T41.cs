using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationAccountName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationBranchName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecieverName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceAccountName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceBranchName",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationAccountName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "DestinationBranchName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "RecieverName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "SourceAccountName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "SourceBranchName",
                table: "Transfers");
        }
    }
}
