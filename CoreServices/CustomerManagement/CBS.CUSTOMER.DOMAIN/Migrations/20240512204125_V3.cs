using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DateOfEstablishment",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCategoryId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EconomicActivitiesId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDNumberIssueAt",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDNumberIssueDate",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Income",
                table: "Groups",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipApprovalStatus",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POBox",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfBirth",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingStatus",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CustomerCategoryId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "EconomicActivitiesId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IDNumberIssueAt",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IDNumberIssueDate",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Income",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MembershipApprovalStatus",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "POBox",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PlaceOfBirth",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "WorkingStatus",
                table: "Groups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfEstablishment",
                table: "Groups",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
