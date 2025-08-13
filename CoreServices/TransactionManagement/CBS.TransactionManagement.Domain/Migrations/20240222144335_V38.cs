using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Tellers_TellerId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TellerId",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryProvisionHistoryId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryProvisionHistoryId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TellerId",
                table: "Accounts",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Tellers_TellerId",
                table: "Accounts",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id");
        }
    }
}
