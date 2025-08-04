using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class M18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CashReplenishmentUsbTeller",
                table: "CashReplenishmentUsbTeller");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CashReplenishmentPrimaryTeller",
                table: "CashReplenishmentPrimaryTeller");

            migrationBuilder.RenameTable(
                name: "CashReplenishmentUsbTeller",
                newName: "CashReplenishmentSubTellers");

            migrationBuilder.RenameTable(
                name: "CashReplenishmentPrimaryTeller",
                newName: "CashReplenishmentPrimaryTellers");

            migrationBuilder.RenameColumn(
                name: "ReplenishmentReferenceNumber",
                table: "CashReplenishmentPrimaryTellers",
                newName: "TransactionReference");

            migrationBuilder.RenameColumn(
                name: "ProvisionedDate",
                table: "CashReplenishmentPrimaryTellers",
                newName: "InitializeDate");

            migrationBuilder.RenameColumn(
                name: "ProvisionedBy",
                table: "CashReplenishmentPrimaryTellers",
                newName: "RequesterUserId");

            migrationBuilder.RenameColumn(
                name: "InitiationDate",
                table: "CashReplenishmentPrimaryTellers",
                newName: "ApprovedDate");

            migrationBuilder.RenameColumn(
                name: "InitiatedBy",
                table: "CashReplenishmentPrimaryTellers",
                newName: "RequesterName");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "CashReplenishmentPrimaryTellers",
                newName: "RequestedAmount");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedComment",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedStatus",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAmount",
                table: "CashReplenishmentPrimaryTellers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Requetcomment",
                table: "CashReplenishmentPrimaryTellers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CashReplenishmentSubTellers",
                table: "CashReplenishmentSubTellers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CashReplenishmentPrimaryTellers",
                table: "CashReplenishmentPrimaryTellers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CashReplenishmentSubTellers",
                table: "CashReplenishmentSubTellers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CashReplenishmentPrimaryTellers",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "ApprovedComment",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "ApprovedStatus",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "ConfirmedAmount",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.DropColumn(
                name: "Requetcomment",
                table: "CashReplenishmentPrimaryTellers");

            migrationBuilder.RenameTable(
                name: "CashReplenishmentSubTellers",
                newName: "CashReplenishmentUsbTeller");

            migrationBuilder.RenameTable(
                name: "CashReplenishmentPrimaryTellers",
                newName: "CashReplenishmentPrimaryTeller");

            migrationBuilder.RenameColumn(
                name: "TransactionReference",
                table: "CashReplenishmentPrimaryTeller",
                newName: "ReplenishmentReferenceNumber");

            migrationBuilder.RenameColumn(
                name: "RequesterUserId",
                table: "CashReplenishmentPrimaryTeller",
                newName: "ProvisionedBy");

            migrationBuilder.RenameColumn(
                name: "RequesterName",
                table: "CashReplenishmentPrimaryTeller",
                newName: "InitiatedBy");

            migrationBuilder.RenameColumn(
                name: "RequestedAmount",
                table: "CashReplenishmentPrimaryTeller",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "InitializeDate",
                table: "CashReplenishmentPrimaryTeller",
                newName: "ProvisionedDate");

            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "CashReplenishmentPrimaryTeller",
                newName: "InitiationDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CashReplenishmentUsbTeller",
                table: "CashReplenishmentUsbTeller",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CashReplenishmentPrimaryTeller",
                table: "CashReplenishmentPrimaryTeller",
                column: "Id");
        }
    }
}
