using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SubTellerProvioningHistories_TellerId",
                table: "SubTellerProvioningHistories",
                column: "TellerId");

            migrationBuilder.CreateIndex(
                name: "IX_PrimaryTellerProvisioningHistories_TellerId",
                table: "PrimaryTellerProvisioningHistories",
                column: "TellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrimaryTellerProvisioningHistories_Tellers_TellerId",
                table: "PrimaryTellerProvisioningHistories",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTellerProvioningHistories_Tellers_TellerId",
                table: "SubTellerProvioningHistories",
                column: "TellerId",
                principalTable: "Tellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrimaryTellerProvisioningHistories_Tellers_TellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTellerProvioningHistories_Tellers_TellerId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropIndex(
                name: "IX_SubTellerProvioningHistories_TellerId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropIndex(
                name: "IX_PrimaryTellerProvisioningHistories_TellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
