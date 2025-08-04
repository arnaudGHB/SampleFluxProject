using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class M19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "PhoneNumberChangeHistories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ChangedByName",
                table: "PhoneNumberChangeHistories",
                newName: "RequestUserId");

            migrationBuilder.RenameColumn(
                name: "ChangeDate",
                table: "PhoneNumberChangeHistories",
                newName: "DateOfRequest");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalComment",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalUserId",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "PhoneNumberChangeHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestBy",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestComment",
                table: "PhoneNumberChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AndriodVersionConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApkUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppSecretcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndriodVersionConfigurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AndriodVersionConfigurations");

            migrationBuilder.DropColumn(
                name: "ApprovalComment",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "ApprovalUserId",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "RequestBy",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.DropColumn(
                name: "RequestComment",
                table: "PhoneNumberChangeHistories");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PhoneNumberChangeHistories",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "RequestUserId",
                table: "PhoneNumberChangeHistories",
                newName: "ChangedByName");

            migrationBuilder.RenameColumn(
                name: "DateOfRequest",
                table: "PhoneNumberChangeHistories",
                newName: "ChangeDate");
        }
    }
}
