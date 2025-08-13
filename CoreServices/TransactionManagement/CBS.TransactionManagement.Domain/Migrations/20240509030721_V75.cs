using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V75 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberAccountActivationPolicyId",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "AccountNumberToPeformWithdrawal",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "CustomerNotification",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "NotificationDate",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "NotificationEndDate",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "NotifyBeforeWithdrawal",
                table: "MemberAccountActivations");

            migrationBuilder.RenameColumn(
                name: "ReopeningFee",
                table: "MemberAccountActivations",
                newName: "LoanPolicyFee");

            migrationBuilder.RenameColumn(
                name: "RegistrationFee",
                table: "MemberAccountActivations",
                newName: "EntranceFee");

            migrationBuilder.RenameColumn(
                name: "MemberAccountActivationPolicyId",
                table: "MemberAccountActivations",
                newName: "MemberRegistrationFeePolicyId");

            migrationBuilder.RenameColumn(
                name: "ClossingFee",
                table: "MemberAccountActivations",
                newName: "ByeLawFee");

            migrationBuilder.RenameIndex(
                name: "IX_MemberAccountActivations_MemberAccountActivationPolicyId",
                table: "MemberAccountActivations",
                newName: "IX_MemberAccountActivations_MemberRegistrationFeePolicyId");

            migrationBuilder.RenameColumn(
                name: "MinimumReopeningFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumLoanPolicyFee");

            migrationBuilder.RenameColumn(
                name: "MinimumRegistrationFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumEntranceFee");

            migrationBuilder.RenameColumn(
                name: "MinimumAccountClossingFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumByeLawsFee");

            migrationBuilder.RenameColumn(
                name: "MaximumReopeningFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumBuildingContributionFee");

            migrationBuilder.RenameColumn(
                name: "MaximumRegistrationFee",
                table: "MemberAccountActivationPolicies",
                newName: "MaximumLoanPolicyFee");

            migrationBuilder.RenameColumn(
                name: "MaximumAccountClossingFee",
                table: "MemberAccountActivationPolicies",
                newName: "MaximumEntrancenFee");

            migrationBuilder.AddColumn<decimal>(
                name: "BuildingContribution",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventCodeBuildingContributionFee",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventCodeByeLawsFee",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventCodeEntranceFee",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventCodeLoanPolicyFee",
                table: "MemberAccountActivationPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumBuildingContribution",
                table: "MemberAccountActivationPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumByeLawsFee",
                table: "MemberAccountActivationPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                column: "MemberRegistrationFeePolicyId",
                principalTable: "MemberAccountActivationPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "BuildingContribution",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "EventCodeBuildingContributionFee",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "EventCodeByeLawsFee",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "EventCodeEntranceFee",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "EventCodeLoanPolicyFee",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "MaximumBuildingContribution",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.DropColumn(
                name: "MaximumByeLawsFee",
                table: "MemberAccountActivationPolicies");

            migrationBuilder.RenameColumn(
                name: "MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                newName: "MemberAccountActivationPolicyId");

            migrationBuilder.RenameColumn(
                name: "LoanPolicyFee",
                table: "MemberAccountActivations",
                newName: "ReopeningFee");

            migrationBuilder.RenameColumn(
                name: "EntranceFee",
                table: "MemberAccountActivations",
                newName: "RegistrationFee");

            migrationBuilder.RenameColumn(
                name: "ByeLawFee",
                table: "MemberAccountActivations",
                newName: "ClossingFee");

            migrationBuilder.RenameIndex(
                name: "IX_MemberAccountActivations_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                newName: "IX_MemberAccountActivations_MemberAccountActivationPolicyId");

            migrationBuilder.RenameColumn(
                name: "MinimumLoanPolicyFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumReopeningFee");

            migrationBuilder.RenameColumn(
                name: "MinimumEntranceFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumRegistrationFee");

            migrationBuilder.RenameColumn(
                name: "MinimumByeLawsFee",
                table: "MemberAccountActivationPolicies",
                newName: "MinimumAccountClossingFee");

            migrationBuilder.RenameColumn(
                name: "MinimumBuildingContributionFee",
                table: "MemberAccountActivationPolicies",
                newName: "MaximumReopeningFee");

            migrationBuilder.RenameColumn(
                name: "MaximumLoanPolicyFee",
                table: "MemberAccountActivationPolicies",
                newName: "MaximumRegistrationFee");

            migrationBuilder.RenameColumn(
                name: "MaximumEntrancenFee",
                table: "MemberAccountActivationPolicies",
                newName: "MaximumAccountClossingFee");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumberToPeformWithdrawal",
                table: "MemberAccountActivations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNotification",
                table: "MemberAccountActivations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationDate",
                table: "MemberAccountActivations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationEndDate",
                table: "MemberAccountActivations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "NotifyBeforeWithdrawal",
                table: "MemberAccountActivations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberAccountActivationPolicyId",
                table: "MemberAccountActivations",
                column: "MemberAccountActivationPolicyId",
                principalTable: "MemberAccountActivationPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
