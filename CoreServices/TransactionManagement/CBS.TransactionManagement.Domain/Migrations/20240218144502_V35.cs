using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCashReplenished",
                table: "SubTellerProvioningHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ReplenishedAmount",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CashReplenishmentAmount",
                table: "PrimaryTellerProvisioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsCashReplenishment",
                table: "PrimaryTellerProvisioningHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReplenishmentReferenceNumber",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCashReplenished",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ReplenishedAmount",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "CashReplenishmentAmount",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "IsCashReplenishment",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ReplenishmentReferenceNumber",
                table: "PrimaryTellerProvisioningHistories");
        }
    }
}
