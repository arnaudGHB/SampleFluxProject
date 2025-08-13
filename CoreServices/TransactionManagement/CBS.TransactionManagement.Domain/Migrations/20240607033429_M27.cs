using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloseOfReferenceId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseOfDayReferenceId",
                table: "PrimaryTellerProvisioningHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseOfReferenceId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "CloseOfDayReferenceId",
                table: "PrimaryTellerProvisioningHistories");
        }
    }
}
