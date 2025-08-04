using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperationType",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PerformCashIn",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerformCashOut",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerformTransfer",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TellerType",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationType",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "PerformCashIn",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "PerformCashOut",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "PerformTransfer",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "TellerType",
                table: "Tellers");
        }
    }
}
