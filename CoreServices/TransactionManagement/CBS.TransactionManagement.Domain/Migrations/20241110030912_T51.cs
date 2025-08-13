using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T51 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryBalance",
                table: "GeneralDailyDashboards",
                newName: "PrimaryTillBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "CashReplenishmentPrimaryTill",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CashReplenishmentSubTill",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashReplenishmentPrimaryTill",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "CashReplenishmentSubTill",
                table: "GeneralDailyDashboards");

            migrationBuilder.RenameColumn(
                name: "PrimaryTillBalance",
                table: "GeneralDailyDashboards",
                newName: "PrimaryBalance");
        }
    }
}
