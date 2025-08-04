using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.FileDownloadInfoP;
using CBS.NLoan.Data.Entity.AlertProfileP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.CustomerLoanAccountP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Data.Entity.InterestCalculationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.PeriodP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Data.Entity.WriteOffLoanP;
using Microsoft.EntityFrameworkCore;

namespace CBS.NLoan.Domain.Context
{
    public class LoanContext : DbContext
    {
        public LoanContext(DbContextOptions<LoanContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefundDetail>()
                .HasOne(rd => rd.Refund)
                .WithMany(r => r.RefundDetails)
                .HasForeignKey(rd => rd.RefundId)
                .OnDelete(DeleteBehavior.Restrict); // Specify that cascade delete should not occur

            modelBuilder.Entity<LoanApplicationCollateral>()
          .HasOne(lc => lc.LoanProductCollateral)
          .WithMany()
          .HasForeignKey(lc => lc.LoanProductCollateralId)
          .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction if you prefer
        }
        public virtual DbSet<LoanApplication> LoanApplications { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanProduct> LoanProducts { get; set; }
        public virtual DbSet<DailyInterestCalculation> DailyInterestCalculations { get; set; }
        public virtual DbSet<FileDownloadInfo> FileDownloadInfo { get; set; }
        public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public virtual DbSet<OTPNotification> OTPNotifications { get; set; }
        public virtual DbSet<LoanNotificationSetting> LoanNotificationSettings { get; set; }
        public virtual DbSet<LoanAmortization> LoanAmortizations { get; set; }
        public virtual DbSet<LoanDeliquencyConfiguration> LoanDeliquencyConfigurations { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<DisburstedLoan> DisburstedLoans { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<LoanTerm> LoanTerms { get; set; }
        
        public virtual DbSet<DocumentPack> DocumentPacks { get; set; }
        public virtual DbSet<DocumentJoin> DocumentJoins { get; set; }

        public virtual DbSet<Refund> Refunds { get; set; }
        public virtual DbSet<RefundDetail> RefundDetails { get; set; }

        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<RescheduleLoan> RescheduleLoans { get; set; }
        public virtual DbSet<LoanGuarantor> LoanGuarantors { get; set; }
        public virtual DbSet<LoanCommentry> LoanCommentries { get; set; }
        public virtual DbSet<LineOfCredit> LineOfCredits { get; set; }

        public virtual DbSet<Collateral> Collaterals { get; set; }
        public virtual DbSet<LoanApplicationCollateral> LoanCollaterals { get; set; }

        public virtual DbSet<LoanCommiteeValidationHistory> CreditCommiteeValidations { get; set; }
        public virtual DbSet<LoanCommiteeGroup> LoanCommiteeGroups { get; set; }
        public virtual DbSet<LoanCommiteeMember> LoanCommeteeMembers { get; set; }
        public virtual DbSet<CustomerLoanAccount> CustomerLoanAccounts { get; set; }
        public virtual DbSet<FeeRange> FeeRanges { get; set; }
        public virtual DbSet<LoanApplicationFee> LoanApplicationFee { get; set; }
        public virtual DbSet<DocumentAttachedToLoan> DocumentAttachedToLoans { get; set; }

        public virtual DbSet<Fee> Fees { get; set; }

        public virtual DbSet<FundingLine> FundingLines { get; set; }

        public virtual DbSet<WriteOffLoan> WriteOffLoans { get; set; }

        public virtual DbSet<LoanPurpose> LoanPurposes { get; set; }
        public virtual DbSet<AlertProfile> AlertProfiles { get; set; }
        public virtual DbSet<Penalty> Penalties { get; set; }
        public virtual DbSet<LoanProductCategory> LoanProductCategories { get; set; }
        public virtual DbSet<LoanProductRepaymentOrder> LoanProductRepaymentOrder { get; set; }
        public virtual DbSet<LoanProductRepaymentCycle> LoanProductRepaymentCycle { get; set; }
        public virtual DbSet<LoanProductMaturityPeriodExtension> LoanProductMaturityPeriodExtensions { get; set; }
    }
}
