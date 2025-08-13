using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T70 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Leader",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin1",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin10",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin100",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin25",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin5",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin50",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningCoin500",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningNote1000",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningNote10000",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningNote2000",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningNote500",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "OpeningNote5000",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "ActualBalance",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "OperationDirection",
                table: "CashCeillingMovements");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "CashCeillingMovements",
                newName: "TransactionReference");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "CashCeillingMovements",
                newName: "RequestedBy");

            migrationBuilder.RenameColumn(
                name: "PreviouseBalance",
                table: "CashCeillingMovements",
                newName: "CashoutRequestAmount");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "CashCeillingMovements",
                newName: "InitializeDate");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "CashCeillingMovements",
                newName: "Requetcomment");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "CashCeillingMovements",
                newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "AccountingDate",
                table: "CashCeillingMovements",
                newName: "ApprovedDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "VaultAuthorisedPersons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "CashCeillingMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedComment",
                table: "CashCeillingMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedStatus",
                table: "CashCeillingMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "CashCeillingMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "CashCeillingMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "CashCeillingMovements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TellerId",
                table: "CashCeillingMovements",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CashCeillingMovements_TellerId",
                table: "CashCeillingMovements",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashCeillingMovements_Tellers_TellerId",
                table: "CashCeillingMovements",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashCeillingMovements_Tellers_TellerId",
                table: "CashCeillingMovements");

            migrationBuilder.DropIndex(
                name: "IX_CashCeillingMovements_TellerId",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "VaultAuthorisedPersons");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "ApprovedComment",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "ApprovedStatus",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CashCeillingMovements");

            migrationBuilder.DropColumn(
                name: "TellerId",
                table: "CashCeillingMovements");

            migrationBuilder.RenameColumn(
                name: "TransactionReference",
                table: "CashCeillingMovements",
                newName: "Source");

            migrationBuilder.RenameColumn(
                name: "Requetcomment",
                table: "CashCeillingMovements",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "RequestedBy",
                table: "CashCeillingMovements",
                newName: "Reference");

            migrationBuilder.RenameColumn(
                name: "RequestType",
                table: "CashCeillingMovements",
                newName: "BankId");

            migrationBuilder.RenameColumn(
                name: "InitializeDate",
                table: "CashCeillingMovements",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "CashoutRequestAmount",
                table: "CashCeillingMovements",
                newName: "PreviouseBalance");

            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "CashCeillingMovements",
                newName: "AccountingDate");

            migrationBuilder.AddColumn<string>(
                name: "Leader",
                table: "Vaults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin1",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin10",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin100",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin25",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin5",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin50",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin500",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote1000",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote10000",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote2000",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote500",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote5000",
                table: "Vaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualBalance",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OperationDirection",
                table: "CashCeillingMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
