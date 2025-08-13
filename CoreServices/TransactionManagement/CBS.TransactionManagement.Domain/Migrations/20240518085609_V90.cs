using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargesToAppliedConsideringClosingTheAccount",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "ChargesToAppliedWithinNormalPeriod",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "CloseOfAccountCharge",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForClossingAccountFee",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "EventAttributForWithdrawalFormFee",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "MaximumWithdrawalRateToConsiderAccountClossed",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "MinimumWithdrawalRateToConsiderAccountRunning",
                table: "WithdrawalParameters");

            migrationBuilder.DropColumn(
                name: "MoralPersonWithdrawalFormFee",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "PhysicalPersonWithdrawalFormFee",
                table: "SavingProducts");

            migrationBuilder.RenameColumn(
                name: "WithdrawalFormCharge",
                table: "WithdrawalParameters",
                newName: "PhysicalPersonWithdrawalFormFee");

            migrationBuilder.RenameColumn(
                name: "WithdrawalChargeWithoutNotification",
                table: "WithdrawalParameters",
                newName: "MoralPersonWithdrawalFormFee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhysicalPersonWithdrawalFormFee",
                table: "WithdrawalParameters",
                newName: "WithdrawalFormCharge");

            migrationBuilder.RenameColumn(
                name: "MoralPersonWithdrawalFormFee",
                table: "WithdrawalParameters",
                newName: "WithdrawalChargeWithoutNotification");

            migrationBuilder.AddColumn<decimal>(
                name: "ChargesToAppliedConsideringClosingTheAccount",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargesToAppliedWithinNormalPeriod",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CloseOfAccountCharge",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForClossingAccountFee",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventAttributForWithdrawalFormFee",
                table: "WithdrawalParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumWithdrawalRateToConsiderAccountClossed",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumWithdrawalRateToConsiderAccountRunning",
                table: "WithdrawalParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
    }
}
