using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chartOfAccountId",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "isDebit",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "operationEventAttributeId",
                table: "Tellers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "chartOfAccountId",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDebit",
                table: "Tellers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "operationEventAttributeId",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
