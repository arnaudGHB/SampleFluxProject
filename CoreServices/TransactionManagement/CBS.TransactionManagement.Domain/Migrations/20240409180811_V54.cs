using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CurrencyNotesId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CurrencyNotesId",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyNotesId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CurrencyNotesId",
                table: "Transactions",
                column: "CurrencyNotesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CurrencyNotes_CurrencyNotesId",
                table: "Transactions",
                column: "CurrencyNotesId",
                principalTable: "CurrencyNotes",
                principalColumn: "Id");
        }
    }
}
