using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastOPerationAmount",
                table: "SubTellerProvioningHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LastOperationType",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInterestCalculatedDate",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastOperation",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastOperationAmount",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOPerationAmount",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "TellerAccountBalance",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "LastInterestCalculatedDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastOperation",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastOperationAmount",
                table: "Accounts");
        }
    }
}
