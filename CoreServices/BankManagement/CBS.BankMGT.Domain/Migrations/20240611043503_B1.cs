using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class B1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerServiceContact",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerServiceContact",
                table: "Banks");
        }
    }
}
