using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashIn",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCashOut",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfCashIn",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfCashOut",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfCashRecieved",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfCashTransfered",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OperationFeeType",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "FeePolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "FeePolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCashIn",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "NumberOfCashOut",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "VolumeOfCashIn",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "VolumeOfCashOut",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "VolumeOfCashRecieved",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "VolumeOfCashTransfered",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "OperationFeeType",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "FeePolicies");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "FeePolicies");
        }
    }
}
