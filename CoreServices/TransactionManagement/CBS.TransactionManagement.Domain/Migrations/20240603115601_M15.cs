using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitiatedBy",
                table: "CashReplenishmentPrimaryTeller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InitiationDate",
                table: "CashReplenishmentPrimaryTeller",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "CashReplenishmentPrimaryTeller",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatedBy",
                table: "CashReplenishmentPrimaryTeller");

            migrationBuilder.DropColumn(
                name: "InitiationDate",
                table: "CashReplenishmentPrimaryTeller");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CashReplenishmentPrimaryTeller");
        }
    }
}
