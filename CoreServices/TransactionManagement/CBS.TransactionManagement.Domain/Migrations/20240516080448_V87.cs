using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdvanceOfSalaryFormFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventCodeAdvanceOfSalaryFormFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventCodeMoralPersonWithdrawalFormFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventCodePhysicalPersonWithdrawalFormFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MoralPersonWithdrawalFormFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhysicalPersonWithdrawalFormFee",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceOfSalaryFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "EventCodeAdvanceOfSalaryFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "EventCodeMoralPersonWithdrawalFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "EventCodePhysicalPersonWithdrawalFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "MoralPersonWithdrawalFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "PhysicalPersonWithdrawalFormFee",
                table: "SavingProducts");
        }
    }
}
