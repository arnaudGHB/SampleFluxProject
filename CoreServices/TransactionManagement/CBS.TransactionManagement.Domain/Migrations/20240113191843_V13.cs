using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TellerProvisioningAccounts",
                table: "TellerProvisioningAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TellerHistory",
                table: "TellerHistory");

            migrationBuilder.RenameTable(
                name: "TellerProvisioningAccounts",
                newName: "TellerProvisioningStatuses");

            migrationBuilder.RenameTable(
                name: "TellerHistory",
                newName: "PrimaryTellerProvisioningHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TellerProvisioningStatuses",
                table: "TellerProvisioningStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrimaryTellerProvisioningHistories",
                table: "PrimaryTellerProvisioningHistories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TellerProvisioningStatuses",
                table: "TellerProvisioningStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrimaryTellerProvisioningHistories",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.RenameTable(
                name: "TellerProvisioningStatuses",
                newName: "TellerProvisioningAccounts");

            migrationBuilder.RenameTable(
                name: "PrimaryTellerProvisioningHistories",
                newName: "TellerHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TellerProvisioningAccounts",
                table: "TellerProvisioningAccounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TellerHistory",
                table: "TellerHistory",
                column: "Id");
        }
    }
}
