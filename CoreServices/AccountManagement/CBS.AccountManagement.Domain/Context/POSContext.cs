using CBS.AccountManagement.Data;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;
using CBS.AccountManagement.Data.Entity;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;

namespace CBS.AccountManagement.Domain
{
    public class POSContext : DbContext
    {
        private string connectionString { get; set; }
        public POSContext(DbContextOptions<POSContext> options, IConfiguration configuration) : base(options)
        {

            //connectionString = configuration.GetRequiredSection("ConnectionStrings")?["CBSAccountManagementDB"] ?? "default_connection_string";
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region sequences
            //     modelBuilder.Entity<AccountingEntryRecordSequence>()
            //.Property(e => e.SequenceDate)
            //.HasColumnType("DATE");
            //     modelBuilder.Entity<AccountingEntryRecordSequence>(entity =>
            //     {
            //         entity.HasKey(s => new { s.SequenceName, s.BranchCode, s.SequenceDate });

            //         entity.Property(s => s.SequenceName)
            //             .HasMaxLength(100);

            //         entity.Property(s => s.BranchCode)
            //             .HasMaxLength(50);

            //         entity.Property(s => s.CurrentValue)
            //             .HasMaxLength(100);
            //     }); 
            #endregion


            modelBuilder.Entity<PostedEntry>()
                .Property(e => e.EntryDetail)
                .HasConversion(
                    itemData => JsonConvert.SerializeObject(itemData), // Serialize to JSON
                    str => JsonConvert.DeserializeObject<JToken>(str) // Deserialize from JSON
                );
            modelBuilder.Entity<ChartOfAccount>()
             .HasMany(p => p.ChartOfAccountManagementPositions)
             .WithOne(c => c.ChartOfAccount)
             .HasForeignKey(c => c.ChartOfAccountId);
            // Define relationships here
            modelBuilder.Entity<AccountingRuleEntry>()
                  .HasOne(a => a.OperationEventAttribute)
                  .WithMany()
                  .HasForeignKey(a => a.OperationEventAttributeId)
                  .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.NoAction

            modelBuilder.Entity<AccountingRuleEntry>()
                .HasOne(a => a.DeterminationAccount)
                .WithMany()
                .HasForeignKey(a => a.DeterminationAccountId)
                .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.NoAction

            modelBuilder.Entity<AccountingRuleEntry>()
                .HasOne(a => a.BalancingAccount)
                .WithMany()
                .HasForeignKey(a => a.BalancingAccountId)
                .OnDelete(DeleteBehavior.NoAction);// or DeleteBehavior.NoAction

            modelBuilder.Entity<TransactionReversalRequestData>()
                      .Property(e => e.ReversalRequest)
                      .HasMaxLength(2500);
            modelBuilder.Entity<TransactionReversalRequestData>()
            .Property(e => e.DataBeforeReversal)
            .HasMaxLength(2500);
            modelBuilder.Entity<TransactionReversalRequestData>()
              .Property(e => e.DataAfterReversal)
              .HasMaxLength(2500);

            modelBuilder.Entity<DocumentReferenceCode>()
           .HasOne(d => d.Document)
           .WithMany(p => p.ReferenceCodes)
           .HasForeignKey(d => d.DocumentId)
           .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.Restrict

       

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Other configuration code...

            optionsBuilder.EnableSensitiveDataLogging();

        }
        //
        public DbSet<ReportDownload> ReportDownloads { get; set; }
        public DbSet<TransactionTracker> TransactionTrackers { get; set; }
        public DbSet<UsersNotification> UsersNotifications { get; set; }
        public DbSet<CashMovementTracker> CashMovementTrackers { get; set; }
        public DbSet<CashMovementTrackingConfiguration> CashMovementTrackingConfigurations { get; set; }
        
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<DepositNotification> DepositNotifications { get; set; }
        public DbSet<AccountPolicy> AccountPolicies { get; set; }
        public DbSet<TrialBalance4column> TrialBalance4column { get; set; }
        public DbSet<TrialBalance> TrialBalance { get; set; }
        public DbSet<EntryTempData> EntryTempData { get; set; }
        public DbSet<TrailBalanceUploud> TrailBalanceUplouds { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<PostedEntry> PostedEntries { get; set; }
        public DbSet<StatementModel> StatementModels { get; set; }
        public DbSet<TrialBalanceReference> TrialBalanceReferences { get; set; }
        public DbSet<BudgetCategory> BudgetCategory { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<ProductAccountingBook> ProductAccountingBook { get; set; }
        public DbSet<OperationEvent> OperationEvent { get; set; }

        public DbSet<CashReplenishment> CashReplenishments { get; set; }
        public DbSet<OperationEventAttributes> OperationEventAttributes { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<ChartOfAccountManagementPosition> ChartOfAccountManagementPositions { get; set; }
        public DbSet<BudgetPeriod> BudgetPeriods { get; set; }
        public DbSet<OrganizationalUnit> OrganizationalUnits { get; set; }
        public DbSet<TransactionReversalRequestData> TransactionReversalRequestData { get; set; }
        public DbSet<AccountClass> AccountClasses { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<TransactionReversalRequest> TransactionReversalRequests { get; set; }
        public DbSet<AccountingRule> AccountingRules { get; set; }
        public DbSet<AccountingRuleEntry> AccountingRuleEntries { get; set; }
        public DbSet<AccountingEntry> AccountingEntries { get; set; }
        public DbSet<TellerDailyProvision> TellerDailyProvisions { get; set; }
        public DbSet<TransactionData> TransactionData { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        //public DbSet<FiscalYear> FiscalYears { get; set; }
        public DbSet<CashMovementTracker> ChartOfAccounts { get; set; }
        public DbSet<BudgetItemDetail> BudgetItemDetails { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<BalanceSheetAccount> BalanceSheetAccountDtos { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<TrialBalanceFile> TrialBalanceFiles { get; set; }
        public DbSet<DocumentReferenceCode> DocumentReferenceCodes { get; set; }
        public DbSet<CorrespondingMapping> CorrespondingMappings { get; set; }
        public DbSet<CorrespondingMappingException> CorrespondingMappingExceptions { get; set; }
        
            public DbSet<ConditionalAccountReferenceFinancialReport> ConditionalAccountReferenceFinancialReports { get; set; }
    }
}