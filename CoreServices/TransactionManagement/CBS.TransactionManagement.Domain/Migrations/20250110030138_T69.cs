using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T69 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TellerId",
                table: "CashChangeHistories",
                newName: "Reference");

            migrationBuilder.CreateTable(
                name: "Vaults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diamention = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaximumCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviouseBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnryptedBalance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Leader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastOperationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastOperation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningNote10000 = table.Column<int>(type: "int", nullable: false),
                    OpeningNote5000 = table.Column<int>(type: "int", nullable: false),
                    OpeningNote2000 = table.Column<int>(type: "int", nullable: false),
                    OpeningNote1000 = table.Column<int>(type: "int", nullable: false),
                    OpeningNote500 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin500 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin100 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin50 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin25 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin10 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin5 = table.Column<int>(type: "int", nullable: false),
                    OpeningCoin1 = table.Column<int>(type: "int", nullable: false),
                    ClosingNote10000 = table.Column<int>(type: "int", nullable: false),
                    ClosingNote5000 = table.Column<int>(type: "int", nullable: false),
                    ClosingNote2000 = table.Column<int>(type: "int", nullable: false),
                    ClosingNote1000 = table.Column<int>(type: "int", nullable: false),
                    ClosingNote500 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin500 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin100 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin50 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin25 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin10 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin5 = table.Column<int>(type: "int", nullable: false),
                    ClosingCoin1 = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Vaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultAuthorisedPersons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_VaultAuthorisedPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultAuthorisedPersons_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaultOperations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VaultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoneBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_VaultOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultOperations_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaultAuthorisedPersons_VaultId",
                table: "VaultAuthorisedPersons",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultOperations_VaultId",
                table: "VaultOperations",
                column: "VaultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaultAuthorisedPersons");

            migrationBuilder.DropTable(
                name: "VaultOperations");

            migrationBuilder.DropTable(
                name: "Vaults");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "CashChangeHistories",
                newName: "TellerId");
        }
    }
}
