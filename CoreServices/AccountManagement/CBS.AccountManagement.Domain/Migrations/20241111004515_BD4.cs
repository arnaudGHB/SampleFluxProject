using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.AccountManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class BD4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorrespondingMappings_ChartOfAccount_ChartOfAccountId",
                table: "CorrespondingMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_CorrespondingMappings_ChartOfAccount_ChartOfAccountId",
                table: "CorrespondingMappings",
                column: "ChartOfAccountId",
                principalTable: "ChartOfAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorrespondingMappings_ChartOfAccount_ChartOfAccountId",
                table: "CorrespondingMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_CorrespondingMappings_ChartOfAccount_ChartOfAccountId",
                table: "CorrespondingMappings",
                column: "ChartOfAccountId",
                principalTable: "ChartOfAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
