using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdClossingFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestCommissionAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestLiassonAccount",
                table: "SavingProducts");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdRepoeningFee",
                table: "SavingProducts",
                newName: "ChartOfAccountIdPricipalAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdPricipalSavingAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdLiassonAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdManagementFee",
                table: "SavingProducts",
                newName: "ChartOfAccountIdInterestExpenseAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdInterestSavingExpenseAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdInterestAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdInterestSavingAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdCommissionAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdPricipalAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdRepoeningFee");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdLiassonAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdPricipalSavingAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdInterestExpenseAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdManagementFee");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdInterestAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdInterestSavingExpenseAccount");

            migrationBuilder.RenameColumn(
                name: "ChartOfAccountIdCommissionAccount",
                table: "SavingProducts",
                newName: "ChartOfAccountIdInterestSavingAccount");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdClossingFee",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestCommissionAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestLiassonAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
