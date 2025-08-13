using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinAlertBalance",
                table: "Tellers",
                newName: "MinAmount");

            migrationBuilder.RenameColumn(
                name: "MaxAlertBalance",
                table: "Tellers",
                newName: "MaxAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinAmount",
                table: "Tellers",
                newName: "MinAlertBalance");

            migrationBuilder.RenameColumn(
                name: "MaxAmount",
                table: "Tellers",
                newName: "MaxAlertBalance");
        }
    }
}
