using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfRemiitanceInitiated",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRemiitanceReception",
                table: "GeneralDailyDashboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfRemiitanceInitiated",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeOfRemiitanceReception",
                table: "GeneralDailyDashboards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Remittances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceBranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceBranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceTellerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceBranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderSecreteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderCNI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceTellerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverCNI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingBranchCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingBranchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingBranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingTellerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingTellerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceBranchCommision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecivingBranchCommision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitiationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfCashOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePaidToCashDesk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemittanceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Remittances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Remittances");

            migrationBuilder.DropColumn(
                name: "NumberOfRemiitanceInitiated",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "NumberOfRemiitanceReception",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "VolumeOfRemiitanceInitiated",
                table: "GeneralDailyDashboards");

            migrationBuilder.DropColumn(
                name: "VolumeOfRemiitanceReception",
                table: "GeneralDailyDashboards");
        }
    }
}
