using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class VaultCashMovementCommand : IRequest<ServiceResponse<bool>>
    {
        public string Reference { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public string OperationType { get; set; }//Cashin or Cashout
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }

}
