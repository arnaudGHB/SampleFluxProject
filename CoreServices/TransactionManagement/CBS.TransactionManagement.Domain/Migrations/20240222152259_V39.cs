using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryProvisionHistoryId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTellerId",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryTellerId",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryProvisionHistoryId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
