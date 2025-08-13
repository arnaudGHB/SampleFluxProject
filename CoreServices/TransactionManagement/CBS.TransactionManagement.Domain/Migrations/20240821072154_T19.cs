using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin1",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin10",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin100",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin25",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin5",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin50",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin500",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote1000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote10000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote2000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote500",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote5000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "LastOPerationAmount",
                table: "PrimaryTellerProvisioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LastOperationType",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin1",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin10",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin100",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin25",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin5",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin50",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin500",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote1000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote10000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote2000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote500",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote5000",
                table: "PrimaryTellerProvisioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingCoin1",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin10",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin100",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin25",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin5",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin50",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin500",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote1000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote10000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote2000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote500",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote5000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "LastOPerationAmount",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin1",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin10",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin100",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin25",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin5",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin50",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin500",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote1000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote10000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote2000",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote500",
                table: "PrimaryTellerProvisioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote5000",
                table: "PrimaryTellerProvisioningHistories");
        }
    }
}
