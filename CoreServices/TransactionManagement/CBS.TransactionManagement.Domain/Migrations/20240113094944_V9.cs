using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Tellers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tellers",
                newName: "InUsedByUserId");

            migrationBuilder.AddColumn<bool>(
                name: "ActiveStatus",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InUseStatus",
                table: "Tellers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SubTellerProvioningHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TellerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIDInChargeOfTeller = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisionedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClossedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenOfDayAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountInHand = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndOfDayAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountDifference = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviouseBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryTellerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTellerProvioningHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TellerProvisioningAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TellerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIDInChargeOfTeller = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisionedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryTeller = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TellerProvisioningAccounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTellerProvioningHistories");

            migrationBuilder.DropTable(
                name: "TellerProvisioningAccounts");

            migrationBuilder.DropColumn(
                name: "ActiveStatus",
                table: "Tellers");

            migrationBuilder.DropColumn(
                name: "InUseStatus",
                table: "Tellers");

            migrationBuilder.RenameColumn(
                name: "InUsedByUserId",
                table: "Tellers",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Tellers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Tellers",
                type: "datetime2",
                nullable: true);
        }
    }
}
