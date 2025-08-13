using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class VaultTranferCommand : IRequest<ServiceResponse<bool>>
    {
        public string Reference { get; set; }
        public string FromBranchId { get; set; }
        public string ToBranchId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }

    }

}
