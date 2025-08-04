using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class pop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "ThirdPartyBranche");

   

            migrationBuilder.RenameColumn(
                name: "HeadOffice",
                table: "ThirdPartyInstitutions",
                newName: "HeadOfficeTelephone");

            migrationBuilder.RenameColumn(
                name: "TownName",
                table: "ThirdPartyBranche",
                newName: "Name");

           
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ThirdPartyInstitutions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadOfficeLocation",
                table: "ThirdPartyInstitutions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TownId",
                table: "ThirdPartyBranche",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranche",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranche",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranches_CorrespondingBankId",
                table: "ThirdPartyBranche",
                column: "CorrespondingBankId");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranches_TownId",
                table: "ThirdPartyBranche",
                column: "TownId");

      
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_CorrespondingBankId",
                table: "ThirdPartyBranche");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_Towns_TownId",
                table: "ThirdPartyBranche");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranches_CorrespondingBankId",
                table: "ThirdPartyBranche");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranches_TownId",
                table: "ThirdPartyBranche");



            migrationBuilder.DropColumn(
                name: "Email",
                table: "ThirdPartyInstitutions");

            migrationBuilder.DropColumn(
                name: "HeadOfficeLocation",
                table: "ThirdPartyInstitutions");

            migrationBuilder.DropColumn(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranche");

            migrationBuilder.RenameTable(
                name: "Divisions",
                newName: "Division");

            migrationBuilder.RenameColumn(
                name: "HeadOfficeTelephone",
                table: "ThirdPartyInstitutions",
                newName: "HeadOffice");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ThirdPartyBranche",
                newName: "TownName");

           

            migrationBuilder.AlterColumn<string>(
                name: "TownId",
                table: "ThirdPartyBranche",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranche",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "ThirdPartyBranche",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
