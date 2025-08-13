using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Accounts",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyNotesId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions",
                column: "CurrencyNotesId",
                principalTable: "CurrencyNotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Accounts",
                newName: "status");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyNotesId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions",
                column: "CurrencyNotesId",
                principalTable: "CurrencyNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
