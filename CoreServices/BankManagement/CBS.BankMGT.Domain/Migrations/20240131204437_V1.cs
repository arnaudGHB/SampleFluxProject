using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankInitial",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Capital",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfCreation",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeadOfficeAddress",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeadOfficeTelehoneNumber",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImmatriculationNumber",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motto",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PBox",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxPayerNUmber",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebSite",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankInitial",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Capital",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfCreation",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImmatriculationNumber",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motto",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PBox",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxPayerNUmber",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebSite",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankInitial",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Capital",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "HeadOfficeAddress",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "HeadOfficeTelehoneNumber",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ImmatriculationNumber",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Motto",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "PBox",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "TaxPayerNUmber",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "WebSite",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "BankInitial",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Capital",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "ImmatriculationNumber",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Motto",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "PBox",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "TaxPayerNUmber",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "WebSite",
                table: "Banks");
        }
    }
}
