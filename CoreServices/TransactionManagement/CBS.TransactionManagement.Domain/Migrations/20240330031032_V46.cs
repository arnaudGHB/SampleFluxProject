using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BalanceBroughtForward",
                table: "Transactions",
                newName: "SourceBranchCommission");

            migrationBuilder.RenameColumn(
                name: "MinAmount",
                table: "Tellers",
                newName: "MinimumWithdrawalAmount");

            migrationBuilder.RenameColumn(
                name: "MaxAmount",
                table: "Tellers",
                newName: "MinimumTransferAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DestinationBrachId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchCommission",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterBrachOperation",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SourceBrachId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumAmountToManage",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumDepositAmount",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumTransferAmount",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumWithdrawalAmount",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumAmountToManage",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumDepositAmount",
                table: "Tellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DestinationBrachId",
                table: "TellerOperation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationBranchCommission",
                table: "TellerOperation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsInterBranch",
                table: "TellerOperation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceBranchCommission",
                table: "TellerOperation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SourceBranchId",
                table: "TellerOperation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DestinationBrachId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DestinationBranchCommission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsInterBrachOperation",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SourceBrachId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "MaximumAmountToManage",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MaximumDepositAmount",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MaximumTransferAmount",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MaximumWithdrawalAmount",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MinimumAmountToManage",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "MinimumDepositAmount",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "DestinationBrachId",
                table: "TellerOperation");

            migrationBuilder.DropColumn(
                name: "DestinationBranchCommission",
                table: "TellerOperation");

            migrationBuilder.DropColumn(
                name: "IsInterBranch",
                table: "TellerOperation");

            migrationBuilder.DropColumn(
                name: "SourceBranchCommission",
                table: "TellerOperation");

            migrationBuilder.DropColumn(
                name: "SourceBranchId",
                table: "TellerOperation");

            migrationBuilder.RenameColumn(
                name: "SourceBranchCommission",
                table: "Transactions",
                newName: "BalanceBroughtForward");

            migrationBuilder.RenameColumn(
                name: "MinimumWithdrawalAmount",
                table: "Tellers",
                newName: "MinAmount");

            migrationBuilder.RenameColumn(
                name: "MinimumTransferAmount",
                table: "Tellers",
                newName: "MaxAmount");
        }
    }
}
