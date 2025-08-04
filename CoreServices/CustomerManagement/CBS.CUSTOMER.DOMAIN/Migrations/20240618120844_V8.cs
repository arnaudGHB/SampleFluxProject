using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "resolved",
                table: "UploadedCustomerWithErrors",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resolved",
                table: "UploadedCustomerWithErrors");
        }
    }
}
