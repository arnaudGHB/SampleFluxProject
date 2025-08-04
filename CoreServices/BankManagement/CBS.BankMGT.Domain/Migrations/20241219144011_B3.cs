using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class B3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Division_Regions_RegionId",
                table: "Division");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdivisions_Division_DivisionId",
                table: "Subdivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranches_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Division",
                table: "Division");

            migrationBuilder.RenameTable(
                name: "Division",
                newName: "Divisions");

            migrationBuilder.RenameColumn(
                name: "HeadOffice",
                table: "ThirdPartyInstitutions",
                newName: "HeadOfficeTelephone");

            migrationBuilder.RenameIndex(
                name: "IX_Division_RegionId",
                table: "Divisions",
                newName: "IX_Divisions_RegionId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ThirdPartyInstitutions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeadOfficeLocation",
                table: "ThirdPartyInstitutions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrolationId",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranches_CorrespondingBankId",
                table: "ThirdPartyBranches",
                column: "CorrespondingBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Divisions_Regions_RegionId",
                table: "Divisions",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdivisions_Divisions_DivisionId",
                table: "Subdivisions",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_CorrespondingBankId",
                table: "ThirdPartyBranches",
                column: "CorrespondingBankId",
                principalTable: "ThirdPartyInstitutions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_Regions_RegionId",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdivisions_Divisions_DivisionId",
                table: "Subdivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranches_CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ThirdPartyInstitutions");

            migrationBuilder.DropColumn(
                name: "HeadOfficeLocation",
                table: "ThirdPartyInstitutions");

            migrationBuilder.DropColumn(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropColumn(
                name: "CorrolationId",
                table: "AuditTrails");

            migrationBuilder.RenameTable(
                name: "Divisions",
                newName: "Division");

            migrationBuilder.RenameColumn(
                name: "HeadOfficeTelephone",
                table: "ThirdPartyInstitutions",
                newName: "HeadOffice");

            migrationBuilder.RenameIndex(
                name: "IX_Divisions_RegionId",
                table: "Division",
                newName: "IX_Division_RegionId");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Division",
                table: "Division",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranches_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                column: "ThirdPartyInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Regions_RegionId",
                table: "Division",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdivisions_Division_DivisionId",
                table: "Subdivisions",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                column: "ThirdPartyInstitutionId",
                principalTable: "ThirdPartyInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
