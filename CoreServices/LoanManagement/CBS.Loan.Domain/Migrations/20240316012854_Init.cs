using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.NLoan.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Msisdn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    IsSupperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    ServiceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AlertProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collaterals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Collaterals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLoanAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousBalance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLoanId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CustomerLoanAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPacks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_DocumentPacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkDoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Fees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundingLines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountingRuleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_FundingLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineOfCredits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountMin = table.Column<double>(type: "float", nullable: false),
                    AmountMax = table.Column<double>(type: "float", nullable: false),
                    NumberInstallmentMin = table.Column<int>(type: "int", nullable: false),
                    NumberInstallmentMax = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineOfCredits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanCommiteeGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfMembers = table.Column<int>(type: "int", nullable: false),
                    NumberToApprovalsToValidationALoan = table.Column<int>(type: "int", nullable: false),
                    MinimumLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommiteeLeaderUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LoanCommiteeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanCycles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CycleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanDeliquencyConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DaysFrom = table.Column<int>(type: "int", nullable: false),
                    DaysTo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionToPerform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendSMStoClient = table.Column<bool>(type: "bit", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountingRuleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanDeliquencyConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanPurposes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PurposeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanPurposes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Periods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppliedWhenLoanRequestIsGreaterThanSaving = table.Column<bool>(type: "bit", nullable: false),
                    AppliedOnInterest = table.Column<bool>(type: "bit", nullable: false),
                    SavingControlAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Taxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentJoins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentPackId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_DocumentJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentJoins_DocumentPacks_DocumentPackId",
                        column: x => x.DocumentPackId,
                        principalTable: "DocumentPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentJoins_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanCommeteeMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanCommiteeGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanCommeteeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanCommeteeMembers_LoanCommiteeGroups_LoanCommiteeGroupId",
                        column: x => x.LoanCommiteeGroupId,
                        principalTable: "LoanCommiteeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanProducts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanInterestPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanInterestType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinimumInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDurationPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinimumDurationPeriod = table.Column<int>(type: "int", nullable: false),
                    MaximumDurationPeriod = table.Column<int>(type: "int", nullable: false),
                    RequiresGuarantor = table.Column<bool>(type: "bit", nullable: false),
                    DefaultDurationsPeriod = table.Column<int>(type: "int", nullable: false),
                    MinimumNumberOfRepayment = table.Column<int>(type: "int", nullable: false),
                    MaximumNumberOfRepayment = table.Column<int>(type: "int", nullable: false),
                    DefaultNumberOfRepayment = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LoanMinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanMaximumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumCollateralPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumCollateralPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultCollateralPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumCreditInsurancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumCreditInsurancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultCreditInsurancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    MinimumProcessingFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumProcessingFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultProcessingFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasTopUp = table.Column<bool>(type: "bit", nullable: false),
                    MaxTopUpLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinTopUpLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TopUpAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsEarlyPartialRepaymentFeeRate = table.Column<bool>(type: "bit", nullable: false),
                    EarlyPartialRepaymentFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EarlyTotalRepaymentFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsEarlyTotalRepaymentFeeRate = table.Column<bool>(type: "bit", nullable: false),
                    FirstRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculateInterestOnEachRepaymentOnProRatabase = table.Column<bool>(type: "bit", nullable: false),
                    HowShoudInterestBeCahrgedInLoanSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowShoudPrincipalBeCahrgedInLoanSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanScheduleDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForPrincipalAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForAccrualInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForPenalty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForFee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForTax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForWriteOffPotfolio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForInterestIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForWriteOffInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForLoanLossReserve = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionOnPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionReversalOnPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForLoanLossReserveInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionOnInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionReversalOnInterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForLoanLossReservePenalties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionOnLateFees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForProvisionReversalOnLateFees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForEarlyPartialRepaymentFeeIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartOfAccountIdForEarlyTotalRepaymentFeeIncome = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_LoanProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProducts_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerProfileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_CustomerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerProfiles_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsThereGuarantor = table.Column<bool>(type: "bit", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfRepayment = table.Column<int>(type: "int", nullable: false),
                    RepaymentCircle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanDuration = table.Column<int>(type: "int", nullable: false),
                    FirstInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EconomicActivityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisburstmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GracePeriod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsuranceCoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollateralCoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShareAccountCoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SavingAccountCoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GuaratorAccountCoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsGuaranteeProvided = table.Column<bool>(type: "bit", nullable: false),
                    IsGuarantorProvided = table.Column<bool>(type: "bit", nullable: false),
                    IsCollateralProvided = table.Column<bool>(type: "bit", nullable: false),
                    LoanPurposeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalLoanRiskCoverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDisbursed = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApplications_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanApplications_LoanPurposes_LoanPurposeId",
                        column: x => x.LoanPurposeId,
                        principalTable: "LoanPurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanApplications_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoanProductFeeJoins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_LoanProductFeeJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductFeeJoins_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanProductFeeJoins_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanProductMaturityPeriodExtensions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternLoanAfterMaturityPeriod = table.Column<bool>(type: "bit", nullable: false),
                    MaturityPeriodLoanInterestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculateInterestOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestRate = table.Column<double>(type: "float", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecurringPeriod = table.Column<int>(type: "int", nullable: false),
                    RecurringPeriodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncludePenalTyFee = table.Column<bool>(type: "bit", nullable: false),
                    KeepLoanStatusAsPAssedMaturityEvenAfterLoanIsExterneded = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LoanProductMaturityPeriodExtensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductMaturityPeriodExtensions_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanProductRepaymentCycle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepaymentCycle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_LoanProductRepaymentCycle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductRepaymentCycle_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanProductRepaymentOrder",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepaymentOrder = table.Column<int>(type: "int", nullable: false),
                    RepaymentReceive = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_LoanProductRepaymentOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductRepaymentOrder_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Penalties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PenaltyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PenaltyValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRate = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CalculatePenaltyOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaivePenaltyOnBranchHolidays = table.Column<bool>(type: "bit", nullable: false),
                    GracePeriodInDaysBeforeApplyingPenalty = table.Column<int>(type: "int", nullable: false),
                    RecuringInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecurringPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysToApplyPenalty = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalties_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCommiteeValidations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanCommiteeMemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CreditCommiteeValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCommiteeValidations_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCommiteeValidations_LoanCommeteeMembers_LoanCommiteeMemberId",
                        column: x => x.LoanCommiteeMemberId,
                        principalTable: "LoanCommeteeMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAttachedToLoans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_DocumentAttachedToLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAttachedToLoans_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAttachedToLoans_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanCollaterals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ValueRated = table.Column<double>(type: "float", nullable: false),
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
                    table.PrimaryKey("PK_LoanCollaterals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanCollaterals_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanGuarantors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GuarantorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMemberOfMicrofinance = table.Column<bool>(type: "bit", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanGuarantors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanGuarantors_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestForcasted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccrualInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccrualInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccrualInterestBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrincipalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Penalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLoanDisbursted = table.Column<bool>(type: "bit", nullable: false),
                    LastInterestCalculatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRefundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRestructured = table.Column<bool>(type: "bit", nullable: false),
                    NewLoanId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsWriteOffLoan = table.Column<bool>(type: "bit", nullable: false),
                    IsDeliquentLoan = table.Column<bool>(type: "bit", nullable: false),
                    IsCurrentLoan = table.Column<bool>(type: "bit", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanProductCollateral",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CollateralId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanProductCollateralTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumValueRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumValueRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanApplicationCollateralId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_LoanProductCollateral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanProductCollateral_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanProductCollateral_LoanCollaterals_LoanApplicationCollateralId",
                        column: x => x.LoanApplicationCollateralId,
                        principalTable: "LoanCollaterals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoanProductCollateral_LoanProducts_LoanProductId",
                        column: x => x.LoanProductId,
                        principalTable: "LoanProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanAmortizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sno = table.Column<int>(type: "int", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Penalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Due = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateOfPayment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MethodOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRescheduled = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LoanAmortizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanAmortizations_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanCommentries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_LoanCommentries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanCommentries_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepaymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Penalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfPayment = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RescheduleLoans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_RescheduleLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RescheduleLoans_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffLoans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OutstandingLoanBalance = table.Column<double>(type: "float", nullable: false),
                    AccruedInterests = table.Column<double>(type: "float", nullable: false),
                    AccruedPenalties = table.Column<double>(type: "float", nullable: false),
                    PastDueDays = table.Column<int>(type: "int", nullable: false),
                    OverduePrincipal = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WriteOffMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountingRuleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_WriteOffLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffLoans_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefundId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanAmortizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CollectedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyAmountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_RefundDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundDetails_LoanAmortizations_LoanAmortizationId",
                        column: x => x.LoanAmortizationId,
                        principalTable: "LoanAmortizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundDetails_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCommiteeValidations_LoanApplicationId",
                table: "CreditCommiteeValidations",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCommiteeValidations_LoanCommiteeMemberId",
                table: "CreditCommiteeValidations",
                column: "LoanCommiteeMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfiles_LoanProductId",
                table: "CustomerProfiles",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachedToLoans_DocumentId",
                table: "DocumentAttachedToLoans",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachedToLoans_LoanApplicationId",
                table: "DocumentAttachedToLoans",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentJoins_DocumentId",
                table: "DocumentJoins",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentJoins_DocumentPackId",
                table: "DocumentJoins",
                column: "DocumentPackId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAmortizations_LoanId",
                table: "LoanAmortizations",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_LoanProductId",
                table: "LoanApplications",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_LoanPurposeId",
                table: "LoanApplications",
                column: "LoanPurposeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_TaxId",
                table: "LoanApplications",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanCollaterals_LoanApplicationId",
                table: "LoanCollaterals",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanCommentries_LoanId",
                table: "LoanCommentries",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanCommeteeMembers_LoanCommiteeGroupId",
                table: "LoanCommeteeMembers",
                column: "LoanCommiteeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanGuarantors_LoanApplicationId",
                table: "LoanGuarantors",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductCollateral_CollateralId",
                table: "LoanProductCollateral",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductCollateral_LoanApplicationCollateralId",
                table: "LoanProductCollateral",
                column: "LoanApplicationCollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductCollateral_LoanProductId",
                table: "LoanProductCollateral",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductFeeJoins_FeeId",
                table: "LoanProductFeeJoins",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductFeeJoins_LoanProductId",
                table: "LoanProductFeeJoins",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductMaturityPeriodExtensions_LoanProductId",
                table: "LoanProductMaturityPeriodExtensions",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductRepaymentCycle_LoanProductId",
                table: "LoanProductRepaymentCycle",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProductRepaymentOrder_LoanProductId",
                table: "LoanProductRepaymentOrder",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_TaxId",
                table: "LoanProducts",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanApplicationId",
                table: "Loans",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_LoanProductId",
                table: "Penalties",
                column: "LoanProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_LoanAmortizationId",
                table: "RefundDetails",
                column: "LoanAmortizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_RefundId",
                table: "RefundDetails",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_LoanId",
                table: "Refunds",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_RescheduleLoans_LoanId",
                table: "RescheduleLoans",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffLoans_LoanId",
                table: "WriteOffLoans",
                column: "LoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertProfiles");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CreditCommiteeValidations");

            migrationBuilder.DropTable(
                name: "CustomerLoanAccounts");

            migrationBuilder.DropTable(
                name: "CustomerProfiles");

            migrationBuilder.DropTable(
                name: "DocumentAttachedToLoans");

            migrationBuilder.DropTable(
                name: "DocumentJoins");

            migrationBuilder.DropTable(
                name: "FundingLines");

            migrationBuilder.DropTable(
                name: "LineOfCredits");

            migrationBuilder.DropTable(
                name: "LoanCommentries");

            migrationBuilder.DropTable(
                name: "LoanCycles");

            migrationBuilder.DropTable(
                name: "LoanDeliquencyConfigurations");

            migrationBuilder.DropTable(
                name: "LoanGuarantors");

            migrationBuilder.DropTable(
                name: "LoanProductCollateral");

            migrationBuilder.DropTable(
                name: "LoanProductFeeJoins");

            migrationBuilder.DropTable(
                name: "LoanProductMaturityPeriodExtensions");

            migrationBuilder.DropTable(
                name: "LoanProductRepaymentCycle");

            migrationBuilder.DropTable(
                name: "LoanProductRepaymentOrder");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "RefundDetails");

            migrationBuilder.DropTable(
                name: "RescheduleLoans");

            migrationBuilder.DropTable(
                name: "WriteOffLoans");

            migrationBuilder.DropTable(
                name: "LoanCommeteeMembers");

            migrationBuilder.DropTable(
                name: "DocumentPacks");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Collaterals");

            migrationBuilder.DropTable(
                name: "LoanCollaterals");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "LoanAmortizations");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "LoanCommiteeGroups");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "LoanApplications");

            migrationBuilder.DropTable(
                name: "LoanProducts");

            migrationBuilder.DropTable(
                name: "LoanPurposes");

            migrationBuilder.DropTable(
                name: "Taxes");
        }
    }
}
