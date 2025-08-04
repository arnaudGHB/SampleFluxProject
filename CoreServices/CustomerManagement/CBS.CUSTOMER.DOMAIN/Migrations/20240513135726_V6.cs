using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardSignatureSpecimenDetails");

            migrationBuilder.RenameColumn(
                name: "BranchManagerName",
                table: "CardSignatureSpecimens",
                newName: "SpecialInstruction");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "CardSignatureSpecimens",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "IdentityCardNumber",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instruction",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedAt",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IssuedOn",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl1",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl1",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl2",
                table: "CardSignatureSpecimens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityCardNumber",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "Instruction",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "IssuedOn",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "PhotoUrl1",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "SignatureUrl1",
                table: "CardSignatureSpecimens");

            migrationBuilder.DropColumn(
                name: "SignatureUrl2",
                table: "CardSignatureSpecimens");

            migrationBuilder.RenameColumn(
                name: "SpecialInstruction",
                table: "CardSignatureSpecimens",
                newName: "BranchManagerName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CardSignatureSpecimens",
                newName: "BranchId");

            migrationBuilder.CreateTable(
                name: "CardSignatureSpecimenDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardSignatureSpecimenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentityCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IssuedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSignatureSpecimenDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardSignatureSpecimenDetails_CardSignatureSpecimens_CardSignatureSpecimenId",
                        column: x => x.CardSignatureSpecimenId,
                        principalTable: "CardSignatureSpecimens",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardSignatureSpecimenDetails_CardSignatureSpecimenId",
                table: "CardSignatureSpecimenDetails",
                column: "CardSignatureSpecimenId");
        }
    }
}
