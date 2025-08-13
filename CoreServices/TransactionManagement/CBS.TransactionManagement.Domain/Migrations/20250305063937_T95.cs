using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T95 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HeadOfficeCommision",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsChargesInclussive",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceiverAmount",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RecevingBranchTotalAmount",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadOfficeCommision",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "IsChargesInclussive",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ReceiverAmount",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "RecevingBranchTotalAmount",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Remittances");
        }
    }
}
