using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReversalRequests_MemberAccounts_AccountId",
                table: "ReversalRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReversalRequests_AccountId",
                table: "ReversalRequests");

           

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "ReversalRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AccountUsed",
                table: "ReversalRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountUsed",
                table: "ReversalRequests");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "ReversalRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ReversalRequests_AccountId",
                table: "ReversalRequests",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReversalRequests_MemberAccounts_AccountId",
                table: "ReversalRequests",
                column: "AccountId",
                principalTable: "MemberAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
