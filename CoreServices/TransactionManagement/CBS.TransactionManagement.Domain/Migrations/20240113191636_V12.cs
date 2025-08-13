using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndOfDayCurrencyNotes",
                table: "TellerHistory");

            migrationBuilder.RenameColumn(
                name: "startTime",
                table: "TellerHistory",
                newName: "OpenedDate");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TellerHistory",
                newName: "UserIdInChargeOfThisTeller");

            migrationBuilder.RenameColumn(
                name: "StartOfDayCurrencyNotes",
                table: "TellerHistory",
                newName: "StartOfDayCurrencyNoteId");

            migrationBuilder.RenameColumn(
                name: "StartOfDayAmount",
                table: "TellerHistory",
                newName: "PreviouseBalance");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "TellerHistory",
                newName: "ClossedDate");

            migrationBuilder.RenameColumn(
                name: "TellerID",
                table: "SubTellerProvioningHistories",
                newName: "TellerId");

            migrationBuilder.RenameColumn(
                name: "UserIDInChargeOfTeller",
                table: "SubTellerProvioningHistories",
                newName: "UserIdInChargeOfThisTeller");

            migrationBuilder.RenameColumn(
                name: "AmountInHand",
                table: "SubTellerProvioningHistories",
                newName: "BalanceAtHand");

            migrationBuilder.RenameColumn(
                name: "AmountDifference",
                table: "SubTellerProvioningHistories",
                newName: "AccountBalance");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUserID",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OpenOfDayAmount",
                table: "TellerHistory",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProvisionedBy",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartOfDayCurrencyNoteId",
                table: "SubTellerProvioningHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "LastUserID",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "OpenOfDayAmount",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "ProvisionedBy",
                table: "TellerHistory");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "StartOfDayCurrencyNoteId",
                table: "SubTellerProvioningHistories");

            migrationBuilder.RenameColumn(
                name: "UserIdInChargeOfThisTeller",
                table: "TellerHistory",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StartOfDayCurrencyNoteId",
                table: "TellerHistory",
                newName: "StartOfDayCurrencyNotes");

            migrationBuilder.RenameColumn(
                name: "PreviouseBalance",
                table: "TellerHistory",
                newName: "StartOfDayAmount");

            migrationBuilder.RenameColumn(
                name: "OpenedDate",
                table: "TellerHistory",
                newName: "startTime");

            migrationBuilder.RenameColumn(
                name: "ClossedDate",
                table: "TellerHistory",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "TellerId",
                table: "SubTellerProvioningHistories",
                newName: "TellerID");

            migrationBuilder.RenameColumn(
                name: "UserIdInChargeOfThisTeller",
                table: "SubTellerProvioningHistories",
                newName: "UserIDInChargeOfTeller");

            migrationBuilder.RenameColumn(
                name: "BalanceAtHand",
                table: "SubTellerProvioningHistories",
                newName: "AmountInHand");

            migrationBuilder.RenameColumn(
                name: "AccountBalance",
                table: "SubTellerProvioningHistories",
                newName: "AmountDifference");

            migrationBuilder.AddColumn<string>(
                name: "EndOfDayCurrencyNotes",
                table: "TellerHistory",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
