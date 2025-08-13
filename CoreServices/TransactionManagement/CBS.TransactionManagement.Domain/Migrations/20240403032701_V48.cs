using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TellerInterestBalance",
                table: "Accounts",
                newName: "OpeningBalance");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfLastOperation",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOpeningBalance",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTellerAccount",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LastInterestPosted",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DateOfLastOperation",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DateOfOpeningBalance",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsTellerAccount",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastInterestPosted",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance",
                table: "Accounts",
                newName: "TellerInterestBalance");
        }
    }
}
