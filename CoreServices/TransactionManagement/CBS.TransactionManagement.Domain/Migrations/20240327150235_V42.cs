using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestCommissionAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChartOfAccountIdInterestLiassonAccount",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDepositAccount",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLoanAccount",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMemberShare",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreferenceShare",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSavingAccount",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestCommissionAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ChartOfAccountIdInterestLiassonAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsDepositAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsLoanAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsMemberShare",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsPreferenceShare",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsSavingAccount",
                table: "SavingProducts");
        }
    }
}
