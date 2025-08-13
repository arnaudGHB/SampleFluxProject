using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new SavingProduct.
    /// </summary>
    public class AddSavingProductCommand : IRequest<ServiceResponse<SavingProductDto>>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal MinimumAccountBalancePhysicalPerson { get; set; }
        public decimal MinimumAccountBalanceMoralPerson { get; set; }
        public string ProductCategory { get; set; }//Teller, OrdinaryAccount,Remittance

        public string InterestAccrualFrequency { get; set; }//Daily, Monthly, Yearly, Qaurterly
        public string PostingFrequency { get; set; }
        public bool IsUsedForTellerProvisioning { get; set; }
        public bool IsCapitalizeInterest { get; set; }
        public string CurrencyId { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsTermProduct { get; set; }
        public bool OTPControl { get; set; }
        public bool AutoVerifyRemittanceSender { get; set; }
        public bool AutoVerifyRemittanceReceiver { get; set; }

        public bool IsWithdrawalAllowedDirectlyFromthisAccount { get; set; } = false;
        public bool IsDepositAllowedDirectlyTothisAccount { get; set; } = false;
        public bool AllowInterbranchWithdrawal { get; set; } = false;
        public bool AllowInterbranchDeposit { get; set; } = false;
        public bool AllowInterbranchTransfter { get; set; } = false;
        public bool AllowShareing { get; set; } = false;

        public string? Description { get; set; }
        public string? ChartOfAccountIdPricipalAccount { get; set; }
        public string? ChartOfAccountIdInterestAccount { get; set; }
        public string? ChartOfAccountIdInterestExpenseAccount { get; set; }
        public string? ChartOfAccountIdCommissionAccount { get; set; }
        public string? ChartOfAccountIdLiassonAccount { get; set; }
        public string? ChartOfAccountIdSavingFee { get; set; }
        public string? ChartOfAccountIdWithrawalFee { get; set; }
        public string? ChartOfAccountIdTransferFee { get; set; }
        public string AccountType { get; set; }
        public bool CanPeformTransferMobileApp { get; set; }
        public bool CanPeformCashinMobileApp { get; set; }
        public bool CanPeformCashOutMobileApp { get; set; }
        public bool CanPeformTransfer3PP { get; set; }
        public bool CanPeformCashin3PP { get; set; }
        public bool CanPeformCashOut3PP { get; set; }
        public int DisplayOrder { get; set; }
        public bool ActivateSavingWithdrawalNotificationForMobileApp { get; set; }
        public bool ActivateForMobileApp { get; set; }
        public bool ActivateFor3PPApp { get; set; }
    }

}
