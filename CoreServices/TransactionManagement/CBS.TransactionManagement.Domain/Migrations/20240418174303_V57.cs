using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coin1",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin10",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin100",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin25",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin5",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin50",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Coin500",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Note1000",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Note10000",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Note2000",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Note500",
                table: "CurrencyNotes");

            migrationBuilder.RenameColumn(
                name: "StartOfDayCurrencyNoteId",
                table: "SubTellerProvioningHistories",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "StartOfDayCurrencyNoteId",
                table: "PrimaryTellerProvisioningHistories",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "Note5000",
                table: "CurrencyNotes",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Denomination",
                table: "CurrencyNotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DinominationType",
                table: "CurrencyNotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "CurrencyNotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "CurrencyNotes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CurrencyNotes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Denomination",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "DinominationType",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "CurrencyNotes");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "CurrencyNotes");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "SubTellerProvioningHistories",
                newName: "StartOfDayCurrencyNoteId");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "PrimaryTellerProvisioningHistories",
                newName: "StartOfDayCurrencyNoteId");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CurrencyNotes",
                newName: "Note5000");

            migrationBuilder.AddColumn<int>(
                name: "Coin1",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin10",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin100",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin25",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin5",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin50",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coin500",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Note1000",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Note10000",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Note2000",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Note500",
                table: "CurrencyNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
