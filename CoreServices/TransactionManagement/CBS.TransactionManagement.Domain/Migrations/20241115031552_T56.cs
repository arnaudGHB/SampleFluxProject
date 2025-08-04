using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfInterBranchCashIn",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfInterBranchCashOut",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfInterBranchCashIn",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfInterBranchCashOut",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfInterBranchCashIn",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfInterBranchCashOut",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "VolumeOfInterBranchCashIn",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "VolumeOfInterBranchCashOut",
                table: "GeneralDailyDashboards");
        }
    }
}
