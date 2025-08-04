using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L58 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestAmountUpfront",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "InterestMustBePaidUpFront",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MigrationDate",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InterestMustBePaidUpFront",
                table: "LoanProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestAmountUpfront",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestAmountUpfront",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "InterestMustBePaidUpFront",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "MigrationDate",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "InterestMustBePaidUpFront",
                table: "LoanProducts");

            migrationBuilder.DropColumn(
                name: "InterestAmountUpfront",
                table: "LoanApplications");
        }
    }
}
