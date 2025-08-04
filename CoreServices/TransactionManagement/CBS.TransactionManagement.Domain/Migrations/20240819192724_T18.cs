using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingDate",
                table: "Transfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingDate",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingDate",
                table: "TellerOperation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin1",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin10",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin100",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin25",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin5",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin50",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingCoin500",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote1000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote10000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote2000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote500",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClosingNote5000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin1",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin10",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin100",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin25",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin5",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin50",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningCoin500",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote1000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote10000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote2000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote500",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningNote5000",
                table: "SubTellerProvioningHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccountingDays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    ClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCentralized = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AccountingDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashChangeHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TellerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GivenNote10000 = table.Column<int>(type: "int", nullable: false),
                    GivenNote5000 = table.Column<int>(type: "int", nullable: false),
                    GivenNote2000 = table.Column<int>(type: "int", nullable: false),
                    GivenNote1000 = table.Column<int>(type: "int", nullable: false),
                    GivenNote500 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin500 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin100 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin50 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin25 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin10 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin5 = table.Column<int>(type: "int", nullable: false),
                    GivenCoin1 = table.Column<int>(type: "int", nullable: false),
                    ReceivedNote10000 = table.Column<int>(type: "int", nullable: false),
                    ReceivedNote5000 = table.Column<int>(type: "int", nullable: false),
                    ReceivedNote2000 = table.Column<int>(type: "int", nullable: false),
                    ReceivedNote1000 = table.Column<int>(type: "int", nullable: false),
                    ReceivedNote500 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin500 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin100 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin50 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin25 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin10 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin5 = table.Column<int>(type: "int", nullable: false),
                    ReceivedCoin1 = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CashChangeHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingDays");

            migrationBuilder.DropTable(
                name: "CashChangeHistories");

            migrationBuilder.DropColumn(
                name: "AccountingDate",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "AccountingDate",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AccountingDate",
                table: "TellerOperation");

            migrationBuilder.DropColumn(
                name: "ClosingCoin1",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin10",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin100",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin25",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin5",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin50",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingCoin500",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote1000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote10000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote2000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote500",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "ClosingNote5000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin1",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin10",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin100",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin25",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin5",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin50",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningCoin500",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote1000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote10000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote2000",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote500",
                table: "SubTellerProvioningHistories");

            migrationBuilder.DropColumn(
                name: "OpeningNote5000",
                table: "SubTellerProvioningHistories");
        }
    }
}
