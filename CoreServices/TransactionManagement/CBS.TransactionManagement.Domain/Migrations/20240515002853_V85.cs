using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V85 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "ChargesWaived");

            migrationBuilder.RenameColumn(
                name: "WithdrawalCharge",
                table: "WithdrawalNotifications",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "WithdrawalNotifications",
                newName: "ReasonForWithdrawal");

            migrationBuilder.RenameColumn(
                name: "NotificationCharge",
                table: "WithdrawalNotifications",
                newName: "LoanBalance");

            migrationBuilder.RenameColumn(
                name: "MatrurityDate",
                table: "WithdrawalNotifications",
                newName: "DateOfIntendedWithdrawal");

            migrationBuilder.RenameColumn(
                name: "IsClossed",
                table: "WithdrawalNotifications",
                newName: "IsWithdrawalDone");

            migrationBuilder.RenameColumn(
                name: "AmountToWithdraw",
                table: "WithdrawalNotifications",
                newName: "Loan");

            migrationBuilder.RenameColumn(
                name: "AmountPaied",
                table: "WithdrawalNotifications",
                newName: "Interest");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ChargesWaived",
                newName: "WaiverInitiator");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberBranchId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InitiatingBranchId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalance",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRequired",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "WithdrawalNotifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByName",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedComment",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFormFeeWasPaid",
                table: "WithdrawalNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Fines",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FormNotificationCharge",
                table: "WithdrawalNotifications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ServiceClearName",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TellerCaise",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TellerCaise_fee",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TellerId_fee",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TellerName",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TellerName_fee",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "ChargesWaived",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TellerCaise",
                table: "ChargesWaived",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TellerName",
                table: "ChargesWaived",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalance",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "AmountRequired",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ApprovedByName",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ApprovedComment",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "DateFormFeeWasPaid",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "Fines",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "FormNotificationCharge",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ServiceClearName",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "TellerCaise",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "TellerCaise_fee",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "TellerId_fee",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "TellerName",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "TellerName_fee",
                table: "WithdrawalNotifications");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "ChargesWaived");

            migrationBuilder.DropColumn(
                name: "TellerCaise",
                table: "ChargesWaived");

            migrationBuilder.DropColumn(
                name: "TellerName",
                table: "ChargesWaived");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "WithdrawalNotifications",
                newName: "WithdrawalCharge");

            migrationBuilder.RenameColumn(
                name: "ReasonForWithdrawal",
                table: "WithdrawalNotifications",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "LoanBalance",
                table: "WithdrawalNotifications",
                newName: "NotificationCharge");

            migrationBuilder.RenameColumn(
                name: "Loan",
                table: "WithdrawalNotifications",
                newName: "AmountToWithdraw");

            migrationBuilder.RenameColumn(
                name: "IsWithdrawalDone",
                table: "WithdrawalNotifications",
                newName: "IsClossed");

            migrationBuilder.RenameColumn(
                name: "Interest",
                table: "WithdrawalNotifications",
                newName: "AmountPaied");

            migrationBuilder.RenameColumn(
                name: "DateOfIntendedWithdrawal",
                table: "WithdrawalNotifications",
                newName: "MatrurityDate");

            migrationBuilder.RenameColumn(
                name: "WaiverInitiator",
                table: "ChargesWaived",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "TellerId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberBranchId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InitiatingBranchId",
                table: "WithdrawalNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExpired",
                table: "ChargesWaived",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
