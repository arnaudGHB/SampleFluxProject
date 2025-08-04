using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.LoanDisbursementP.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>

    public class LoanDisbursmentCommand : IRequest<ServiceResponse<bool>>
    {
        public decimal Amount { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string CustomerId { get; set; }
        public string LoanId { get; set; }
        public string Note { get; set; }
        public bool IsNormal { get; set; }
        public string LoanApplicationType { get; set; }
        public decimal RestructuredBalance { get; set; }
        public decimal RequestedAmount { get; set; }
        public bool IsChargeInclussive { get; set; }
        public string LoanProductId { get; set; }
        public OldLoanPayment OldLoanPayment { get; set; }
        public List<ChargCollection>? ChargCollections { get; set; }
    }
    public class OldLoanPayment
    {
        public decimal Amount { get; set; }
        public decimal Capital { get; set; }
        public decimal VAT { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public string LoanId { get; set; }
    }
    public class ChargCollection
    {
        public decimal Amount { get; set; }
        public string? EventCode { get; set; }
        public string? ChargeName { get; set; }
    }
}
