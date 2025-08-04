using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.UserServiceManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBankAndBranchIdFromEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
