using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TellerOperation_Transactions_TransactionID",
                table: "TellerOperation");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "TellerOperation",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "TransactionRef",
                table: "TellerOperation",
                newName: "TransactionReference");

            migrationBuilder.RenameIndex(
                name: "IX_TellerOperation_TransactionID",
                table: "TellerOperation",
                newName: "IX_TellerOperation_TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TellerOperation_Transactions_TransactionId",
                table: "TellerOperation",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TellerOperation_Transactions_TransactionId",
                table: "TellerOperation");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "TellerOperation",
                newName: "TransactionID");

            migrationBuilder.RenameColumn(
                name: "TransactionReference",
                table: "TellerOperation",
                newName: "TransactionRef");

            migrationBuilder.RenameIndex(
                name: "IX_TellerOperation_TransactionId",
                table: "TellerOperation",
                newName: "IX_TellerOperation_TransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TellerOperation_Transactions_TransactionID",
                table: "TellerOperation",
                column: "TransactionID",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
