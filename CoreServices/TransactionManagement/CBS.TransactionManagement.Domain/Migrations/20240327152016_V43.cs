using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCapitalizeInterest",
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

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "SavingProducts");

            migrationBuilder.AddColumn<bool>(
                name: "IsCapitalizeInterest",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
    }
}
