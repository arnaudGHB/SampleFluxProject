using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProductCollateral_LoanCollaterals_LoanApplicationCollateralId",
                table: "LoanProductCollateral");

            migrationBuilder.DropIndex(
                name: "IX_LoanProductCollateral_LoanApplicationCollateralId",
                table: "LoanProductCollateral");

            migrationBuilder.DropColumn(
                name: "LoanApplicationCollateralId",
                table: "LoanProductCollateral");

            migrationBuilder.DropColumn(
                name: "LoanProductCollateralTag",
                table: "LoanProductCollateral");

            migrationBuilder.DropColumn(
                name: "ValueRated",
                table: "LoanCollaterals");

            migrationBuilder.RenameColumn(
                name: "IsMemberOfMicrofinance",
                table: "LoanGuarantors",
                newName: "IsCoMember");

            migrationBuilder.RenameColumn(
                name: "BankAccountNumber",
                table: "LoanGuarantors",
                newName: "AccountNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuaranteeAmount",
                table: "LoanGuarantors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "LoanCollaterals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductCollateralsId",
                table: "LoanCollaterals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "LoanCollaterals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "LoanCollaterals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "LoanCollaterals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Collaterals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralsId",
                table: "LoanCollaterals",
                column: "LoanProductCollateralsId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralsId",
                table: "LoanCollaterals",
                column: "LoanProductCollateralsId",
                principalTable: "LoanProductCollateral",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanCollaterals_LoanProductCollateral_LoanProductCollateralsId",
                table: "LoanCollaterals");

            migrationBuilder.DropIndex(
                name: "IX_LoanCollaterals_LoanProductCollateralsId",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LoanGuarantors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "LoanGuarantors");

            migrationBuilder.DropColumn(
                name: "GuaranteeAmount",
                table: "LoanGuarantors");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "LoanProductCollateralsId",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "LoanCollaterals");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "LoanCollaterals");

            migrationBuilder.RenameColumn(
                name: "IsCoMember",
                table: "LoanGuarantors",
                newName: "IsMemberOfMicrofinance");

            migrationBuilder.RenameColumn(
                name: "AccountNumber",
                table: "LoanGuarantors",
                newName: "BankAccountNumber");

            migrationBuilder.AddColumn<string>(
                name: "LoanApplicationCollateralId",
                table: "LoanProductCollateral",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanProductCollateralTag",
                table: "LoanProductCollateral",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "LoanGuarantors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ValueRated",
                table: "LoanCollaterals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Collaterals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductCollateral_LoanApplicationCollateralId",
                table: "LoanProductCollateral",
                column: "LoanApplicationCollateralId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProductCollateral_LoanCollaterals_LoanApplicationCollateralId",
                table: "LoanProductCollateral",
                column: "LoanApplicationCollateralId",
                principalTable: "LoanCollaterals",
                principalColumn: "Id");
        }
    }
}
