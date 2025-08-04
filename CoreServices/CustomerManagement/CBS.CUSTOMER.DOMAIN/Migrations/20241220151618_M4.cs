using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class M4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivatedBy",
                table: "CMoneyMembersActivationAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivatingBranchCode",
                table: "CMoneyMembersActivationAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivatingBranchId",
                table: "CMoneyMembersActivationAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivatingBranchName",
                table: "CMoneyMembersActivationAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedBy",
                table: "CMoneyMembersActivationAccounts");

            migrationBuilder.DropColumn(
                name: "ActivatingBranchCode",
                table: "CMoneyMembersActivationAccounts");

            migrationBuilder.DropColumn(
                name: "ActivatingBranchId",
                table: "CMoneyMembersActivationAccounts");

            migrationBuilder.DropColumn(
                name: "ActivatingBranchName",
                table: "CMoneyMembersActivationAccounts");
        }
    }
}
