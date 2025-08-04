using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateFeeRangeCommand : IRequest<ServiceResponse<FeeRangeDto>>
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal PercentageValue { get; set; }
        public decimal Charge { get; set; }
    }

}
