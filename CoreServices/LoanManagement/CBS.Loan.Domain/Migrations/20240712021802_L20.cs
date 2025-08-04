using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanDurarion",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "PreviouseBalance",
                table: "DailyInterestCalculations",
                newName: "PreviousBalance");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUpload",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoanDuration",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanDuration",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "PreviousBalance",
                table: "DailyInterestCalculations",
                newName: "PreviouseBalance");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUpload",
                table: "Loans",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "LoanDurarion",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
