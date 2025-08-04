using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfOPeration",
                table: "OtherTransactions",
                newName: "DateOfOperation");

            migrationBuilder.RenameColumn(
                name: "Naration",
                table: "OtherTransactions",
                newName: "TelephoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "EventCode",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CNI",
                table: "OtherTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Narration",
                table: "OtherTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventCode",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "CNI",
                table: "OtherTransactions");

            migrationBuilder.DropColumn(
                name: "Narration",
                table: "OtherTransactions");

            migrationBuilder.RenameColumn(
                name: "DateOfOperation",
                table: "OtherTransactions",
                newName: "DateOfOPeration");

            migrationBuilder.RenameColumn(
                name: "TelephoneNumber",
                table: "OtherTransactions",
                newName: "Naration");
        }
    }
}
