using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T77 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExternalAccount",
                table: "StandingOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalAccountHolderName",
                table: "StandingOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalAccountNumber",
                table: "StandingOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalNote",
                table: "StandingOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalAccount",
                table: "StandingOrders");

            migrationBuilder.DropColumn(
                name: "ExternalAccountHolderName",
                table: "StandingOrders");

            migrationBuilder.DropColumn(
                name: "ExternalAccountNumber",
                table: "StandingOrders");

            migrationBuilder.DropColumn(
                name: "PersonalNote",
                table: "StandingOrders");
        }
    }
}
