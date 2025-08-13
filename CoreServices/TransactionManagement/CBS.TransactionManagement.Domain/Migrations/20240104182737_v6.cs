using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndOfDayCurrencyNotes",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartOfDayCurrencyNotes",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndOfDayCurrencyNotes",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "StartOfDayCurrencyNotes",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "SavingProducts");
        }
    }
}
