using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;

namespace CBS.TransactionManagement.Data
{
    public class SavingProduct:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? AccountNuber { get; set; }
        public bool OTPControl { get; set; }
        public bool AutoVerifyRemittanceSender { get; set; }
        public bool AutoVerifyRemittanceReceiver { get; set; }

        public string? AccountManagementPositionId { get; set; }
        public decimal MinAmount { get; set; } = 0;
        public decimal MaxAmount { get; set; } = 0;
        public decimal MinimumAccountBalancePhysicalPerson { get; set; } = 0;
        public decimal MinimumAccountBalanceMoralPerson { get; set; } = 0;
        public decimal AdvanceOfSalaryFormFee { get; set; } = 0;
        public bool AllowInterbranchDeposit { get; set; } = false;
        public bool AllowInterbranchTransfter { get; set; } = false;
        public bool IsWithdrawalAllowedDirectlyFromthisAccount { get; set; } = false;
        public bool IsDepositAllowedDirectlyTothisAccount { get; set; } = false;
        public bool AllowInterbranchWithdrawal { get; set; } = false;
        public bool AllowShareing { get; set; } = false;
        public bool AutoAddToMember { get; set; } = false;
        public string? EventCodePhysicalPersonWithdrawalFormFee { get; set; }
        public string? EventCodeMoralPersonWithdrawalFormFee { get; set; }
        public string? EventCodeAdvanceOfSalaryFormFee { get; set; }
        public string InterestAccrualFrequency { get; set; }//Daily, Monthly, Yearly, Qaurterly
        public string PostingFrequency { get; set; }
        public string AccountType { get; set; }
        public bool IsUsedForTellerProvisioning { get; set; }
        public string CurrencyId { get; set; }
        public bool ActiveStatus { get; set; }
        public bool CanPeformTransferMobileApp { get; set; }
        public bool CanPeformCashinMobileApp { get; set; }
        public bool CanPeformCashOutMobileApp { get; set; }
        public bool CanPeformTransfer3PP { get; set; }
        public bool CanPeformCashin3PP { get; set; }
        public bool CanPeformCashOut3PP { get; set; }
        public bool ActivateForMobileApp { get; set; }
        public bool ActivateFor3PPApp { get; set; }
        public string? ProductCategory { get; set; }//Teller, OrdinaryAccount,Remittance
        public int DisplayOrder { get; set; }
        public bool ActivateSavingWithdrawalNotificationForMobileApp { get; set; }
        public bool IsTermProduct { get; set; }
        public string? BankId { get; set; }
        public string? Description { get; set; }
        public string? ChartOfAccountIdPricipalAccount { get; set; }
        public string? ChartOfAccountIdInterestAccount { get; set; }
        public string? ChartOfAccountIdInterestExpenseAccount { get; set; }
        public string? ChartOfAccountIdLiassonAccount { get; set; }
        public string? ChartOfAccountIdSavingFee { get; set; }
        public string? ChartOfAccountIdWithrawalFee { get; set; }
        public string? ChartOfAccountIdTransferFee { get; set; }
        public decimal WithdrawalFormSavingFormFeeFor3PP { get; set; }
        public string? EventCodeWithdrawalFormSavingFormFeeFor3PP { get; set; }
        public string? ChartOfAccountIdCashInCommission { get; set; }
        public string? ChartOfAccountIdCashOutCommission { get; set; }
        public string? ChartOfAccountIdHeadOfficeShareCashInCommission { get; set; }
        public string? ChartOfAccountIdFluxAndPTMShareCashInCommission { get; set; }
        public string? ChartOfAccountIdCamCCULShareCashInCommission { get; set; }
        public string? ChartOfAccountIdHeadOfficeShareCashOutCommission { get; set; }
        public string? ChartOfAccountIdFluxAndPTMShareCashOutCommission { get; set; }
        public string? ChartOfAccountIdCamCCULShareCashOutCommission { get; set; }

        public string? ChartOfAccountIdHeadOfficeShareCMoneyTransferCommission { get; set; }
        public string? ChartOfAccountIdFluxAndPTMShareCMoneyTransferCommission { get; set; }
        public string? ChartOfAccountIdCamCCULShareCMoneyTransferCommission { get; set; }
        public string? ChartOfAccountIdSourceCMoneyTransferCommission { get; set; }
        public string? ChartOfAccountIdDestinationCMoneyTransferCommission { get; set; }


        public string? ChartOfAccountIdHeadOfficeShareTransferCommission { get; set; }
        public string? ChartOfAccountIdFluxAndPTMShareTransferCommission { get; set; }
        public string? ChartOfAccountIdCamCCULShareTransferCommission { get; set; }


        public virtual ICollection<CashDepositParameter> CashDepositParameters { get; set; }
        public virtual ICollection<ManagementFeeParameter> ManagementFeeParameters { get; set; }
        public virtual ICollection<WithdrawalParameter> WithdrawalParameters { get; set; }
        public virtual ICollection<EntryFeeParameter> EntryFeeParameters { get; set; }
        public virtual ICollection<CloseFeeParameter> CloseFeeParameters { get; set; }
        public virtual ICollection<ReopenFeeParameter> ReopenFeeParameters { get; set; }
        public virtual ICollection<TermDepositParameter> TermDeposits { get; set; }
        public virtual ICollection<TransferParameter> TransferParameters { get; set; }
        public virtual ICollection<SavingProductFee> SavingProductFee { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }

    }


}

