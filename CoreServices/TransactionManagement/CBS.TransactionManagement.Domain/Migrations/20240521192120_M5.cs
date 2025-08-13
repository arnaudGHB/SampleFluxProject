using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FrontEndAuditTB",
                table: "FrontEndAuditTB");

            migrationBuilder.RenameTable(
                name: "FrontEndAuditTB",
                newName: "FrontEndAuditLoggs");

            migrationBuilder.AddColumn<string>(
                name: "AjaxPostData",
                table: "FrontEndAuditLoggs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrontEndAuditLoggs",
                table: "FrontEndAuditLoggs",
                column: "UsersAuditID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FrontEndAuditLoggs",
                table: "FrontEndAuditLoggs");

            migrationBuilder.DropColumn(
                name: "AjaxPostData",
                table: "FrontEndAuditLoggs");

            migrationBuilder.RenameTable(
                name: "FrontEndAuditLoggs",
                newName: "FrontEndAuditTB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrontEndAuditTB",
                table: "FrontEndAuditTB",
                column: "UsersAuditID");
        }
    }
}
