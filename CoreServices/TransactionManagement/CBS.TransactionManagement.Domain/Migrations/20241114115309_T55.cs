using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MobileMoneyCashOut",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDailyCollectionCashIn",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDailyCollectionCashOut",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTransfer",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OrangeMoneyCashOut",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileMoneyCashOut",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfDailyCollectionCashIn",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfDailyCollectionCashOut",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfTransfer",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "OrangeMoneyCashOut",
                table: "GeneralDailyDashboards");
        }
    }
}
