using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class T29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "AuditLogs",
                newName: "UserFullName");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "AuditLogs",
                newName: "StackTrace");

            migrationBuilder.RenameColumn(
                name: "Changes",
                table: "AuditLogs",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "AuditLogs",
                newName: "LogLevel");

            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ControllerName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ControllerName",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserFullName",
                table: "AuditLogs",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "StackTrace",
                table: "AuditLogs",
                newName: "EntityName");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "AuditLogs",
                newName: "Changes");

            migrationBuilder.RenameColumn(
                name: "LogLevel",
                table: "AuditLogs",
                newName: "Action");
        }
    }
}
