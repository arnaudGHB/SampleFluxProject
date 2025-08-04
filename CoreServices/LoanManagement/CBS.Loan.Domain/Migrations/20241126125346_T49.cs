using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoanProductCategoryId",
                table: "LoanPurposes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LoanProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanPurposes_LoanProductCategoryId",
                table: "LoanPurposes",
                column: "LoanProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanPurposes_LoanProductCategories_LoanProductCategoryId",
                table: "LoanPurposes",
                column: "LoanProductCategoryId",
                principalTable: "LoanProductCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanPurposes_LoanProductCategories_LoanProductCategoryId",
                table: "LoanPurposes");

            migrationBuilder.DropIndex(
                name: "IX_LoanPurposes_LoanProductCategoryId",
                table: "LoanPurposes");

            migrationBuilder.DropColumn(
                name: "LoanProductCategoryId",
                table: "LoanPurposes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LoanProductCategories");
        }
    }
}
