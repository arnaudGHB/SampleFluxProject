using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.UserServiceManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBankIdFromUser3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Users");
        }
    }
}
