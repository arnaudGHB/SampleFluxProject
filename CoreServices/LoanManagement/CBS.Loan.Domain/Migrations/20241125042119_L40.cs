using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoanTermId",
                table: "LoanProducts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoanTerms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinInMonth = table.Column<int>(type: "int", nullable: false),
                    MaxInMonth = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LoanTerms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_LoanTermId",
                table: "LoanProducts",
                column: "LoanTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanProducts_LoanTerms_LoanTermId",
                table: "LoanProducts",
                column: "LoanTermId",
                principalTable: "LoanTerms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanProducts_LoanTerms_LoanTermId",
                table: "LoanProducts");

            migrationBuilder.DropTable(
                name: "LoanTerms");

            migrationBuilder.DropIndex(
                name: "IX_LoanProducts_LoanTermId",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "LoanTermId",
                table: "LoanProducts");
        }
    }
}
