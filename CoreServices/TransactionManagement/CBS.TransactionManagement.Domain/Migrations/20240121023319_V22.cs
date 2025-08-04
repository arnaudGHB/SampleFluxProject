using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TellerId",
                table: "Transactions",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tellers_TellerId",
                table: "Transactions",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tellers_TellerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TellerId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
