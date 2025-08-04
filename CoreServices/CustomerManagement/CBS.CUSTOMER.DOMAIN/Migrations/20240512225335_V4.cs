using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CustomerCategoryId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "EconomicActivitiesId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Email",
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
                name: "Package",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PlaceOfBirth",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SubDivisionId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "WorkingStatus",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "TownId",
                table: "Groups",
                newName: "GroupLeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupLeaderId",
                table: "Groups",
                newName: "TownId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCategoryId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
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
                name: "Email",
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
                name: "Package",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfBirth",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubDivisionId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingStatus",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
