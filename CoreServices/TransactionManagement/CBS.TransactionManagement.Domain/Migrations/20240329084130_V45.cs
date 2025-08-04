using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.TransactionManagement.Domain.Migrations
{
    /// <inheritdoc />
    public partial class V45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberAccountActivationPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumRegistrationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumRegistrationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MinimumAccountClossingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumAccountClossingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumReopeningFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumReopeningFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_MemberAccountActivationPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberAccountActivations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClossingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReopeningFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberAccountActivationPolicyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotifyBeforeWithdrawal = table.Column<bool>(type: "bit", nullable: false),
                    AccountNumberToPeformWithdrawal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerNotification = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_MemberAccountActivations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberAccountActivations_MemberAccountActivationPolicies_MemberAccountActivationPolicyId",
                        column: x => x.MemberAccountActivationPolicyId,
                        principalTable: "MemberAccountActivationPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberAccountActivations_MemberAccountActivationPolicyId",
                table: "MemberAccountActivations",
                column: "MemberAccountActivationPolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberAccountActivations");

            migrationBuilder.DropTable(
                name: "MemberAccountActivationPolicies");
        }
    }
}
