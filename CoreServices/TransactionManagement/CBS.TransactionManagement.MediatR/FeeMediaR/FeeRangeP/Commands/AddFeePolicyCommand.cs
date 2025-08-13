using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FeePolicyP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddFeePolicyCommand : IRequest<ServiceResponse<FeePolicyDto>>
    {
        public string FeeId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal Value { get; set; }
        public decimal Charge { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string? EventCode { get; set; }
        public bool IsCentralised { get; set; }

    }

}
