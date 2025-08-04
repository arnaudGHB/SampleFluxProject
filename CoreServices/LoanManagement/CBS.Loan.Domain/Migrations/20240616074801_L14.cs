using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class L14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingRuleId",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "ActionToPerform",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "LoanJourneyStatus",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SendMail",
                table: "LoanDeliquencyConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendSMS",
                table: "LoanDeliquencyConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanJourneyStatus",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SendMail",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.DropColumn(
                name: "SendSMS",
                table: "LoanDeliquencyConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "AccountingRuleId",
                table: "LoanDeliquencyConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActionToPerform",
                table: "LoanDeliquencyConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "LoanDeliquencyConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "LoanDeliquencyConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "LoanDeliquencyConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
