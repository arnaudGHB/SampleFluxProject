using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountReplenished",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "PrimaryTellerID",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "TellerAccountBalance",
                table: "SubTellerProvioningHistories");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "CashReplenishments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InitializeDate",
                table: "CashReplenishments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TellerId",
                table: "CashReplenishments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "CashReplenishments");

            migrationBuilder.DropColumn(
                name: "InitializeDate",
                table: "CashReplenishments");

            migrationBuilder.DropColumn(
                name: "TellerId",
                table: "CashReplenishments");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountReplenished",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTellerID",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TellerAccountBalance",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
