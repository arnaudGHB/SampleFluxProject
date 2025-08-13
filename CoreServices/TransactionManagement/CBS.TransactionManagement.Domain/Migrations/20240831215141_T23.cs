using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "BuildingContribution",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "ByeLawFee",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "EntranceFee",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Fees");

            migrationBuilder.RenameColumn(
                name: "TotalFee",
                table: "MemberAccountActivations",
                newName: "CustomeAmount");

            migrationBuilder.RenameColumn(
                name: "LoanPolicyFee",
                table: "MemberAccountActivations",
                newName: "Amount");

            migrationBuilder.AlterColumn<string>(
                name: "MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FeeId",
                table: "MemberAccountActivations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeeName",
                table: "MemberAccountActivations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "MemberAccountActivations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "MemberAccountActivations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EventCode",
                table: "FeePolicies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberAccountActivations_FeeId",
                table: "MemberAccountActivations",
                column: "FeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccountActivations_Fees_FeeId",
                table: "MemberAccountActivations",
                column: "FeeId",
                principalTable: "Fees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                column: "MemberRegistrationFeePolicyId",
                principalTable: "MemberAccountActivationPolicies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccountActivations_Fees_FeeId",
                table: "MemberAccountActivations");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations");

            migrationBuilder.DropIndex(
                name: "IX_MemberAccountActivations_FeeId",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "FeeId",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "FeeName",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MemberAccountActivations");

            migrationBuilder.DropColumn(
                name: "EventCode",
                table: "FeePolicies");

            migrationBuilder.RenameColumn(
                name: "CustomeAmount",
                table: "MemberAccountActivations",
                newName: "TotalFee");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "MemberAccountActivations",
                newName: "LoanPolicyFee");

            migrationBuilder.AlterColumn<string>(
                name: "MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BuildingContribution",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ByeLawFee",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EntranceFee",
                table: "MemberAccountActivations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberRegistrationFeePolicyId",
                table: "MemberAccountActivations",
                column: "MemberRegistrationFeePolicyId",
                principalTable: "MemberAccountActivationPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
