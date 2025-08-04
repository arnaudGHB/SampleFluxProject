using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoVerifyRemittanceReceiver",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoVerifyRemittanceSender",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OTPControl",
                table: "SavingProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductCategory",
                table: "SavingProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedOTP",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverAddress",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverCNI",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverCNIDateOfExpiration",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverCNIDateOfIssue",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverCNIPlcaceOfIssue",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverName",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedReceiverPhoneNumber",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CapturedRemittanceAmount",
                table: "Remittances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CapturedRemittanceDate",
                table: "Remittances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedSenderAddress",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedSenderName",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedSenderPhoneNumber",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapturedSenderSecretCode",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoVerifyReceiver",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoVerifySender",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManualVerification",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOTPVerified",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverCNIExpiryDate",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverCNIIssueDate",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverCNIPlaceOfIssue",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverCountry",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderCountry",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoVerifyRemittanceReceiver",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "AutoVerifyRemittanceSender",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "OTPControl",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "ProductCategory",
                table: "SavingProducts");

            migrationBuilder.DropColumn(
                name: "CapturedOTP",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverAddress",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverCNI",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverCNIDateOfExpiration",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverCNIDateOfIssue",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverCNIPlcaceOfIssue",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverName",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedReceiverPhoneNumber",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedRemittanceAmount",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedRemittanceDate",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedSenderAddress",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedSenderName",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedSenderPhoneNumber",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "CapturedSenderSecretCode",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "IsAutoVerifyReceiver",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "IsAutoVerifySender",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "IsManualVerification",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "IsOTPVerified",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ReceiverCNIExpiryDate",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ReceiverCNIIssueDate",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ReceiverCNIPlaceOfIssue",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "ReceiverCountry",
                table: "Remittances");

            migrationBuilder.DropColumn(
                name: "SenderCountry",
                table: "Remittances");
        }
    }
}
