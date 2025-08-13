using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopupMobileMoneyLedgerAccountNumberTo",
                table: "Tellers",
                newName: "ToHeadOfficeFloatAccountNumber_D");

            migrationBuilder.RenameColumn(
                name: "TopupMobileMoneyLedgerAccountNumberFrom",
                table: "Tellers",
                newName: "ToBranchFloatAccountNumberHeadOffice_B");

            migrationBuilder.AddColumn<string>(
                name: "FromAuxillaryAccountNumber_A",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromBranchAccountNumber_C",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromBranchFloatAccountNumber_D",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromHeadOfficeAccountNumber_B",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToBranchFloatAccountNumberAuxillary_A",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToBranchFloatAccountNumberBranch_C",
                table: "Tellers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromAuxillaryAccountNumber_A",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "FromBranchAccountNumber_C",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "FromBranchFloatAccountNumber_D",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "FromHeadOfficeAccountNumber_B",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "ToBranchFloatAccountNumberAuxillary_A",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "ToBranchFloatAccountNumberBranch_C",
                table: "Tellers");

            migrationBuilder.RenameColumn(
                name: "ToHeadOfficeFloatAccountNumber_D",
                table: "Tellers",
                newName: "TopupMobileMoneyLedgerAccountNumberTo");

            migrationBuilder.RenameColumn(
                name: "ToBranchFloatAccountNumberHeadOffice_B",
                table: "Tellers",
                newName: "TopupMobileMoneyLedgerAccountNumberFrom");
        }
    }
}
