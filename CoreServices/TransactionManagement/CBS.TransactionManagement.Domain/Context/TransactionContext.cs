using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.ChargesWaivedP;
using CBS.TransactionManagement.Data.Entity;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Data.Entity.ClossingOfAccountP;
using CBS.TransactionManagement.Data.Entity.DailyStatisticBoard;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.Data.Entity.Receipts.Details;
using CBS.TransactionManagement.Data.Entity.Receipts.Payments;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Data.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using CBS.TransactionManagement.Data.Entity.VaultOperationP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Domain
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
        {
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<GimacPayment> GimacPayments { get; set; }
        public DbSet<CashOutThirdParty> CashOutThirdParty { get; set; }
        public DbSet<AccountingDay> AccountingDays { get; set; }
        public DbSet<CashChangeHistory> CashChangeHistories { get; set; }
        public DbSet<CashDepositParameter> CashDepositParameters { get; set; }
        public DbSet<MobileMoneyCashTopup> MobileMoneyCashTopups { get; set; }
        public DbSet<Account> MemberAccounts { get; set; }
        public DbSet<ReversalRequest> ReversalRequests { get; set; }
        public DbSet<Remittance> Remittances { get; set; }
        public DbSet<SalaryAnalysisResult> SalaryAnalysisResults { get; set; }
        
        public DbSet<SalaryAnalysisResultDetail> SalaryAnalysisResultDetails { get; set; }
        public DbSet<SalaryUploadModel> SalaryUploadModels { get; set; }
        public DbSet<StandingOrder> StandingOrders { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<GeneralDailyDashboard> GeneralDailyDashboards { get; set; }
        public DbSet<SalaryExtract> SalaryExtract { get; set; }
        public DbSet<FileUpload> FileUpload { get; set; }
        public DbSet<CashCeilingRequest> CashCeillingMovements { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<HolyDay> HolyDays { get; set; }
        public DbSet<HolyDayRecurring> HolyDayRecurrings { get; set; }
        public DbSet<OldLoanAccountingMaping> OldLoanAccountingMapings { get; set; }
        public DbSet<FrontEndAuditTB> FrontEndAuditLoggs { get; set; }
        public DbSet<FileDownloadInfo> FileDownloadInfo { get; set; }
        public DbSet<OtherTransaction> OtherTransactions { get; set; }
        public DbSet<SavingProduct> SavingProducts { get; set; }
        public DbSet<WithdrawalParameter> WithdrawalParameters { get; set; }
        public DbSet<TransferParameter> TransferParameters { get; set; }
        public DbSet<EntryFeeParameter> EntryFeeParameters { get; set; }
        public DbSet<ManagementFeeParameter> ManagementFeeParameters { get; set; }
        public DbSet<ReopenFeeParameter> ReopenFeeParameters { get; set; }
        public DbSet<TermDepositParameter> TermDepositParameters { get; set; }
        public DbSet<CloseFeeParameter> CloseFeeParameters { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<VaultOperation> VaultOperations { get; set; }
        public DbSet<VaultAuthorisedPerson> VaultAuthorisedPersons { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<FeePolicy> FeePolicies { get; set; }
        public DbSet<SavingProductFee> SavingProductFees { get; set; }
        public DbSet<WithdrawalNotification> WithdrawalNotifications { get; set; }
        public DbSet<ChargesWaived> ChargesWaived { get; set; }
        public DbSet<ClossingOfAccount> ClossingOfAccounts { get; set; }
        public DbSet<BlockedAccount> BlockedAccounts { get; set; }
        public DbSet<MemberAccountActivation> MemberAccountActivations { get; set; }
        public DbSet<MemberRegistrationFeePolicy> MemberAccountActivationPolicies { get; set; }
        public DbSet<DailyTeller> DailyTellers { get; set; }
        public DbSet<SubTellerProvioningHistory> SubTellerProvioningHistories { get; set; }
        public DbSet<DailyProvision> DailyProvisions { get; set; }
        public DbSet<CurrencyNotes> CurrencyNotes { get; set; }
        public DbSet<Teller> Tellers { get; set; }
        public DbSet<AccountingEvent> AccountingEvents { get; set; }
        public DbSet<PrimaryTellerProvisioningHistory> PrimaryTellerProvisioningHistories { get; set; }
        public DbSet<CashReplenishmentSubTeller> CashReplenishmentSubTellers { get; set; }
        public DbSet<CashReplenishmentPrimaryTeller> CashReplenishmentPrimaryTellers { get; set; }
        public DbSet<MemberNoneCashOperation> MemberNoneCashOperations { get; set; }

    }
}
