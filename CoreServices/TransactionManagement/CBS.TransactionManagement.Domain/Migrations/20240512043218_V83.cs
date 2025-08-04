using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V83 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowInterbranchWithdrawal",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsConfiguredForShareing",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "IsWithdrawalDoneOnthisAccount",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "NumberOfDayToNotifyBeforeWithdrawal",
                table: "WithdrawalParameters");

            migrationBuilder.RenameColumn(
                name: "MinimumWithdrawalRateToConsiderAccountOK",
                table: "WithdrawalParameters",
                newName: "MinimumWithdrawalRateToConsiderAccountRunning");

            migrationBuilder.RenameColumn(
                name: "MaximumRateToConsiderAccountClossed",
                table: "WithdrawalParameters",
                newName: "MaximumWithdrawalRateToConsiderAccountClossed");

            migrationBuilder.RenameColumn(
                name: "EventAttributForDepositFormFee",
                table: "WithdrawalParameters",
                newName: "EventAttributForWithdrawalFormFee");

            migrationBuilder.RenameColumn(
                name: "EventAttributForDepositFee",
                table: "WithdrawalParameters",
                newName: "EventAttributForClossingAccountFee");

            migrationBuilder.AddColumn<bool>(
                name: "AllowInterbranchDeposit",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInterbranchTransfter",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInterbranchWithdrawal",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowShareing",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDepositAllowedDirectlyTothisAccount",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawalAllowedDirectlyFromthisAccount",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowInterbranchDeposit",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "AllowInterbranchTransfter",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "AllowInterbranchWithdrawal",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "AllowShareing",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsDepositAllowedDirectlyTothisAccount",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "IsWithdrawalAllowedDirectlyFromthisAccount",
                table: "SavingProducts");

            migrationBuilder.RenameColumn(
                name: "MinimumWithdrawalRateToConsiderAccountRunning",
                table: "WithdrawalParameters",
                newName: "MinimumWithdrawalRateToConsiderAccountOK");

            migrationBuilder.RenameColumn(
                name: "MaximumWithdrawalRateToConsiderAccountClossed",
                table: "WithdrawalParameters",
                newName: "MaximumRateToConsiderAccountClossed");

            migrationBuilder.RenameColumn(
                name: "EventAttributForWithdrawalFormFee",
                table: "WithdrawalParameters",
                newName: "EventAttributForDepositFormFee");

            migrationBuilder.RenameColumn(
                name: "EventAttributForClossingAccountFee",
                table: "WithdrawalParameters",
                newName: "EventAttributForDepositFee");

            migrationBuilder.AddColumn<bool>(
                name: "AllowInterbranchWithdrawal",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfiguredForShareing",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawalDoneOnthisAccount",
                table: "WithdrawalParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDayToNotifyBeforeWithdrawal",
                table: "WithdrawalParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
