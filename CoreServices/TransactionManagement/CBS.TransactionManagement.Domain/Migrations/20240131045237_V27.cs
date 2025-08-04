using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationEventAttributeId",
                table: "CloseFeeParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeId",
                table: "CashDepositParameters");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeId",
                table: "WithdrawalParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeId",
                table: "TransferParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeId",
                table: "ReopenFeeParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeId",
                table: "ManagementFeeParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeId",
                table: "EntryFeeParameters",
                newName: "OperationEventAttributeName");

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "CloseFeeParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeName",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "CloseFeeParameters");

            migrationBuilder.DropColumn(
                name: "OperationEventAttributeName",
                table: "CashDepositParameters");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "WithdrawalParameters",
                newName: "OperationEventAttributeId");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "TransferParameters",
                newName: "OperationEventAttributeId");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "ReopenFeeParameters",
                newName: "OperationEventAttributeId");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "ManagementFeeParameters",
                newName: "OperationEventAttributeId");

            migrationBuilder.RenameColumn(
                name: "OperationEventAttributeName",
                table: "EntryFeeParameters",
                newName: "OperationEventAttributeId");

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeId",
                table: "CloseFeeParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationEventAttributeId",
                table: "CashDepositParameters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
