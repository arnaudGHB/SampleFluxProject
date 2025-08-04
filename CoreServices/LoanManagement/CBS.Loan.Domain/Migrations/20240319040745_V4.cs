using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralsId",
                table: "LoanCollaterals");

            migrationBuilder.RenameColumn(
                name: "LoanProductCollateralsId",
                table: "LoanCollaterals",
                newName: "LoanProductCollateralId1");

            migrationBuilder.RenameIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralsId",
                table: "LoanCollaterals",
                newName: "IX_LoanCollaterals_LoanProductCollateralId1");

            migrationBuilder.AddColumn<string>(
                name: "LoanProductCollateralId",
                table: "LoanCollaterals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralId",
                table: "LoanCollaterals",
                column: "LoanProductCollateralId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralId",
                table: "LoanCollaterals",
                column: "LoanProductCollateralId",
                principalTable: "LoanProductCollateral",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralId1",
                table: "LoanCollaterals",
                column: "LoanProductCollateralId1",
                principalTable: "LoanProductCollateral",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralId",
                table: "LoanCollaterals");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralId1",
                table: "LoanCollaterals");

            migrationBuilder.DropIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralId",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "LoanProductCollateralId",
                table: "LoanCollaterals");

            migrationBuilder.RenameColumn(
                name: "LoanProductCollateralId1",
                table: "LoanCollaterals",
                newName: "LoanProductCollateralsId");

            migrationBuilder.RenameIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralId1",
                table: "LoanCollaterals",
                newName: "IX_LoanCollaterals_LoanProductCollateralsId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralsId",
                table: "LoanCollaterals",
                column: "LoanProductCollateralsId",
                principalTable: "LoanProductCollateral",
                principalColumn: "Id");
        }
    }
}
