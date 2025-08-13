using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class VaultInitializationCommand : IRequest<ServiceResponse<bool>>
    {
        public bool CanProceed { get; set; }
        public CurrencyNotesRequest CurrencyNote { get; set; }
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }
        public decimal AmountInHand { get; set; }
        public decimal AmountInVault { get; set; }
    }

}
