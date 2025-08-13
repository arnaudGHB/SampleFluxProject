using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T74 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "CashChangeHistories",
                newName: "ServiceOperationType");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountGiven",
                table: "CashChangeHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountReceive",
                table: "CashChangeHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTellerId",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTellerId",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaultId",
                table: "CashChangeHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountGiven",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "AmountReceive",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "PrimaryTellerId",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "SubTellerId",
                table: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "VaultId",
                table: "CashChangeHistories");

            migrationBuilder.RenameColumn(
                name: "ServiceOperationType",
                table: "CashChangeHistories",
                newName: "UserName");
        }
    }
}
