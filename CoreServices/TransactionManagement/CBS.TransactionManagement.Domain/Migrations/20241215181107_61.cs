using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class _61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChargeType",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfIssue",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpirationDate",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "InitailAmount",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfIssue",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SendSMSTotReceiver",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeType",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "DateOfIssue",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "InitailAmount",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "PlaceOfIssue",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "SendSMSTotReceiver",
                table: "Remittances");
        }
    }
}
