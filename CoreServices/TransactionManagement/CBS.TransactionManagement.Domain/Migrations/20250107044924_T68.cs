using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T68 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventCodeWithdrawalFormSavingFormFeeFor3PP",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawalFormSavingFormFeeFor3PP",
                table: "SavingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventCodeWithdrawalFormSavingFormFeeFor3PP",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "WithdrawalFormSavingFormFeeFor3PP",
                table: "SavingProducts");
        }
    }
}
