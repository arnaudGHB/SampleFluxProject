using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V89 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fines",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "Interest",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "Loan",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "WithdrawalNotifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fines",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Interest",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Loan",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
