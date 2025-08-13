using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestPaid",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashInMTN",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashInOrange",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashOutMTN",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashOutOrange",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLoanDisbursementFee",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLoanFee",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Penalties",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestPaid",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfCashInMTN",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfCashInOrange",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfCashOutMTN",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfCashOutOrange",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfLoanDisbursementFee",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfLoanFee",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "Penalties",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "GeneralDailyDashboards");
        }
    }
}
