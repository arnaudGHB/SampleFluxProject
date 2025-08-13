using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FrontEndAuditTBs",
                table: "FrontEndAuditTBs");

            migrationBuilder.RenameTable(
                name: "FrontEndAuditTBs",
                newName: "FrontEndAuditTB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrontEndAuditTB",
                table: "FrontEndAuditTB",
                column: "UsersAuditID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FrontEndAuditTB",
                table: "FrontEndAuditTB");

            migrationBuilder.RenameTable(
                name: "FrontEndAuditTB",
                newName: "FrontEndAuditTBs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrontEndAuditTBs",
                table: "FrontEndAuditTBs",
                column: "UsersAuditID");
        }
    }
}
