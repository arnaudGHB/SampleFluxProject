using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V53 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceAccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceCommision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DestinationCommision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsInterBranchOperation = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatedByUSerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfInitiation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfApproval = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitiatorComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidatorComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TellerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceBrachId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationBrachId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfers_Tellers_TellerId",
                        column: x => x.TellerId,
                        principalTable: "Tellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_AccountId",
                table: "Transfers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TellerId",
                table: "Transfers",
                column: "TellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");
        }
    }
}
