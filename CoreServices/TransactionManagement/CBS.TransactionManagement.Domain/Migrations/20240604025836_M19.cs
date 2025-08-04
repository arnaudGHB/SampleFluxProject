using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "CashReplenishmentSubTellers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CashReplenishmentSubTellers_TellerId",
                table: "CashReplenishmentSubTellers",
                column: "TellerId");

            migrationBuilder.CreateIndex(
                name: "IX_CashReplenishmentPrimaryTellers_TellerId",
                table: "CashReplenishmentPrimaryTellers",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashReplenishmentPrimaryTellers_Tellers_TellerId",
                table: "CashReplenishmentPrimaryTellers",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CashReplenishmentSubTellers_Tellers_TellerId",
                table: "CashReplenishmentSubTellers",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashReplenishmentPrimaryTellers_Tellers_TellerId",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropForeignKey(
                name: "FK_CashReplenishmentSubTellers_Tellers_TellerId",
                table: "CashReplenishmentSubTellers");

            migrationBuilder.DropIndex(
                name: "IX_CashReplenishmentSubTellers_TellerId",
                table: "CashReplenishmentSubTellers");

            migrationBuilder.DropIndex(
                name: "IX_CashReplenishmentPrimaryTellers_TellerId",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "CashReplenishmentSubTellers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
