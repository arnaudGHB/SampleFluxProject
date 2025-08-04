using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OpenningOfDayDate",
                table: "MemberAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenningOfDayReference",
                table: "MemberAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenningOfDayStatus",
                table: "MemberAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenningOfDayDate",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "OpenningOfDayReference",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "OpenningOfDayStatus",
                table: "MemberAccounts");
        }
    }
}
