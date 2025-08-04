using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.BankMGT.Domain.Migrations
{
    /// <inheritdoc />
    public partial class pomp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranche_ThirdPartyInstitutions_CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranche_Towns_TownId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyBranche",
                table: "ThirdPartyBranches");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranche_CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropColumn(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranches");

            migrationBuilder.RenameTable(
                name: "ThirdPartyBranche",
                newName: "ThirdPartyBranches");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyBranche_TownId",
                table: "ThirdPartyBranches",
                newName: "IX_ThirdPartyBranches_TownId");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyBranches",
                table: "ThirdPartyBranches",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranches_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                column: "ThirdPartyInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches",
                column: "ThirdPartyInstitutionId",
                principalTable: "ThirdPartyInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranches_Towns_TownId",
                table: "ThirdPartyBranches",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_ThirdPartyInstitutions_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyBranches_Towns_TownId",
                table: "ThirdPartyBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyBranches",
                table: "ThirdPartyBranches");

            migrationBuilder.DropIndex(
                name: "IX_ThirdPartyBranches_ThirdPartyInstitutionId",
                table: "ThirdPartyBranches");

            migrationBuilder.RenameTable(
                name: "ThirdPartyBranches",
                newName: "ThirdPartyBranches");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyBranches_TownId",
                table: "ThirdPartyBranche",
                newName: "IX_ThirdPartyBranche_TownId");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPartyInstitutionId",
                table: "ThirdPartyBranche",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CorrespondingBankId",
                table: "ThirdPartyBranches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyBranche",
                table: "ThirdPartyBranches",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyBranche_CorrespondingBankId",
                table: "ThirdPartyBranches",
                column: "CorrespondingBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranche_ThirdPartyInstitutions_CorrespondingBankId",
                table: "ThirdPartyBranches",
                column: "CorrespondingBankId",
                principalTable: "ThirdPartyInstitutions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyBranche_Towns_TownId",
                table: "ThirdPartyBranches",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id");
        }
    }
}
